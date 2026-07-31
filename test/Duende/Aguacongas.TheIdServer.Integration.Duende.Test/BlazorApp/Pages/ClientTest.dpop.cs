// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
//
// Pour que ce fichier fonctionne, ajoutez "partial" à la déclaration existante
// dans ClientTest.cs :
//
//   public partial class ClientTest : EntityPageTestBase<ClientPage, Client>
//
// Notes issues de l'analyse de AuthorizeText.razor(.cs) et ClientExtensions.cs :
//
// 1) BUG dans Client.razor : le champ DPoP clock skew utilise
//      <AuthorizeText Name="dpop-clock-skew" @bind-Value="@DPoPClockSkew" />
//    mais AuthorizeText ne déclare que Id/Placeholder/MaxLength (pas de Name).
//    "Name" est donc silencieusement ignoré et l'<input> généré n'a ni id ni
//    name. Je contourne via un sélecteur structurel basé sur le <label for="...">
//    voisin, qui reste valide même si vous corrigez ce bug plus tard.
//    (à corriger côté source : Name="dpop-clock-skew" -> Id="dpop-clock-skew")
//    Cet InputText (hérité par AuthorizeText) réagit à "oninput", pas "onchange" :
//    j'utilise donc TriggerEventAsync("oninput", ...) plutôt que ChangeAsync.
//
// 2) IsWebClient() (ClientExtensions.cs) dépend de HasCustomGrantType(), qui
//    dépend elle-même d'un dictionnaire GrantTypes.Instance que je n'ai pas.
//    Je n'utilise donc plus "client_credentials" comme cas "false" (mon
//    hypothèse précédente était fausse en pratique). A la place j'utilise une
//    liste de grant types VIDE, qui donne IsWebClient() == false de façon
//    certaine quel que soit le contenu de GrantTypes.Instance (Any() sur liste
//    vide = false, et All() sur séquence vide = true donc HasCustomGrantType()
//    = false).
//
// 3) Le composant Filter expose "onfocus" et "onfocusout" (confirmé par le
//    runtime bUnit), pas "onblur". J'utilise donc onfocusout pour simuler la
//    perte de focus.
//
// 4) Client.OnStateChange resynchronise _isWebClient avec la valeur "live" dès
//    qu'une entité est modifiée via HandleModificationState (ajout/suppression
//    d'un grant type via l'UI passe par là). Modifier les grant types par l'UI
//    annulerait donc immédiatement le gel que le test veut démontrer. Pour
//    isoler le comportement de FilterFocusChanged/IsWebClient, je mute
//    Model.AllowedGrantTypes directement par réflexion (sans passer par
//    HandleModificationState) puis force un re-render avec component.Render().
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Bunit;
using Bunit.Extensions.WaitForHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using ClientPage = Aguacongas.TheIdServer.BlazorApp.Pages.Client.Client;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public partial class ClientTest
    {
        private static Client GetModel(ClientPage instance)
        {
            var modelProperty = instance.GetType().GetProperty("Model",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.NotNull(modelProperty);
            return (Client)modelProperty!.GetValue(instance)!;
        }

        [Fact]
        public async Task DPoPValidationModeCheckboxes_should_toggle_bits_independently_and_persist()
        {
            string clientId = await CreateClient();

            // Etat de départ connu (nonce seul) pour prouver l'indépendance des bits.
            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstAsync(c => c.Id == clientId);
                client.DPoPValidationMode = 2;
                await context.SaveChangesAsync();
            });

            var component = CreateComponent("Alice Smith", SharedConstants.WRITERPOLICY, clientId);

            var iatCheckbox = WaitForNode(component, "input[name=\"iat\"]");
            await iatCheckbox.ChangeAsync(new ChangeEventArgs { Value = true });

            var form = component.Find("form");
            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                Assert.Equal(3, client!.DPoPValidationMode); // iat (1) + nonce (2)
            });

            var nonceCheckbox = WaitForNode(component, "input[name=\"nonce\"]");
            await nonceCheckbox.ChangeAsync(new ChangeEventArgs { Value = false });

            form = component.Find("form");
            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                Assert.Equal(1, client!.DPoPValidationMode); // seul iat reste
            });
        }

        [Fact]
        public async Task DPoPClockSkewInput_with_invalid_value_should_not_persist_change()
        {
            string clientId = await CreateClient();
            var component = CreateComponent("Alice Smith", SharedConstants.WRITERPOLICY, clientId);

            var input = WaitForNode(component, "label[for=\"dpop-clock-skew\"] + div input");
            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "not-a-timespan" });

            var form = component.Find("form");
            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                Assert.Equal(TimeSpan.Zero, client!.DPoPClockSkew);
            });
        }

        [Fact]
        public async Task AddClaimButtonClick_should_add_a_new_claim_row()
        {
            string clientId = await CreateClient(); // CreateClient ajoute déjà 1 claim ("filtered")
            var component = CreateComponent("Alice Smith", SharedConstants.WRITERPOLICY, clientId);

            var beforeCount = WaitForAllNodes(component, "#claims input").Count;

            var addButton = WaitForNode(component, "#claims button.btn-sm");
            await addButton.ClickAsync(new MouseEventArgs());

            var afterCount = WaitForAllNodes(component, "#claims input").Count;

            Assert.True(afterCount > beforeCount, "A new empty claim row should have been added.");
        }

        [Fact]
        public async Task FilterFocusChanged_should_freeze_IsWebClient_result_while_filtering()
        {
            // hybrid => IsWebClient() == true de façon certaine (vérifié explicitement
            // dans ClientExtensions.IsWebClient, sans dépendre de GrantTypes.Instance).
            string clientId = await CreateClient("hybrid");
            var component = CreateComponent("Alice Smith", SharedConstants.WRITERPOLICY, clientId);

            WaitForNode(component, "#consent"); // état de départ : client web

            var filterInput = component.Find("input[placeholder=\"filter\"]");
            await filterInput.TriggerEventAsync("onfocus", new FocusEventArgs());

            // Mutation directe du modèle (sans passer par HandleModificationState, qui
            // resynchroniserait _isWebClient via Client.OnStateChange). Une liste de
            // grant types vide donne IsWebClient() == false de façon certaine.
            var model = GetModel(component.Instance);
            model.AllowedGrantTypes = new List<ClientGrantType>();

            component.Render(); // force un nouveau rendu sans passer par HandleModificationState

            // Toujours filtré => la valeur gelée à la prise de focus (true) doit rester active.
            WaitForNode(component, "#consent");

            await filterInput.TriggerEventAsync("onfocusout", new FocusEventArgs());

            // Focus perdu => IsWebClient() est recalculé en direct => plus de grant type => false.
            Assert.Throws<WaitForFailedException>(() => WaitForNode(component, "#consent"));
        }
    }
}
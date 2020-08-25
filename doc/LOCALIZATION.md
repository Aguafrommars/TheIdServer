# Localization

You may localize the Server pages and the admin user interface as needed.

## Cultures

![cultures](assets/cultures.png)  

The default culture is **en** (English). When you add a new culture, a language selector appears in the admin UI.

> A server restart is required to make a new culture active.

You provide translations for each English string resource used by the server or the application on the culture page.

![culture](assets/culture.png)  

The application doesn't provide a list of keys, but instead, a warning log is written each time a key is not found for a specific culture.

In the browser console for application resources
![app key not found sample](assets/app-localized-key-not-found.png)  

In the server log for server resources
![server key not found sample](assets/server-localized-key-not-found.png)  

## Clients, APIs, and Identities

To localize consents and grants screen, you can define localized strings for names and descriptions of your clients, APIs, API's scopes, and identity resources.

![api scope localization sample](assets/api-scope-localization.png)  

And these strings will be localized in consents

![localized consents](assets/localized-consents.png)  

and grants screens.

![localized grants screen](assets/localized-grants.png)  

## Application Welcome screen

When the application setting `welcomeContenUrl` = */api/welcomefragment*

```json
"welcomeContenUrl": "/api/welcomefragment"
```

The API will look for a file named *{EnvironmentName}-welcome-fragment.{CultureName}.html* in the *wwwroot* folder  
and fallback to *{EnvironmentName}-welcome-fragment.html*  
then to *welcome-fragment.{CultureName}.html*  
then to *welcome-fragment.html*

## Culture cookie

Any request to the server containing the query string *culture={CultureName}* sets the culture cookie to the desired culture.

![culture cookie](assets/culture-cookie.png)  

This allow you to get the culture cookie in your apps.

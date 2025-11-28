# 🚀 Deployment Guide: TheIdServer on Render

This guide outlines the steps to deploy **TheIdServer** directly from the public repository using the `render.yaml` blueprint. The process is divided into three stages to ensure the service URL is correctly configured and data persistence is established.

## Prerequisites
* A Render account.

---

## Phase 1: Initial Service Creation (Blueprint)

In this phase, we create the service by connecting directly to the public repository. The application will launch with "placeholder" values.

1.  Log in to the **Render Dashboard**.
2.  Click **New +** and select **Blueprint**.
3.  **Connect the Repository:**
    * Look for the option to connect a **Public Git Repository** or paste the repository URL directly: `https://github.com/Aguafrommars/TheIdServer`
    * Click **Continue**.
4.  **Service Name:** Give your service a name (e.g., `theidserver-prod`).
5.  **Branch:** Ensure the `master` branch (or the branch containing `render.yaml`) is selected.
6.  Click **Apply**.

Render will read the configuration from the repository and trigger the first deployment.

---

## Phase 2: Environment Configuration (URL Correction)

Once the service is created, Render assigns it a public URL (e.g., `https://theidserver-xyz.onrender.com`). You must now update the environment variables to replace the placeholders.

1.  **Copy the URL:** Go to the dashboard of your newly created Web Service and copy the URL from the top left corner.
2.  **Edit Variables:** Go to the **Environment** tab.
3.  **Update Placeholders:** Replace **every occurrence** of `{your web service url}` with your actual copied URL.
    * **Keys to update:**
        * `InitialData__Clients__0__AllowedCorsOrigins__0`
        * `InitialData__Clients__0__ClientUri`
        * `InitialData__Clients__0__PostLogoutRedirectUris__0`
        * `InitialData__Clients__0__RedirectUris__0`
    * **JSON Block Update:** Locate the `APPSETTINGS_JSON` variable. Update the JSON content by replacing `{your web service url}` in:
        * `apiBaseUrl`
        * `providerOptions` (authority, redirectUri, postLogoutRedirectUri)
        * `welcomeContenUrl`
        * `settingsOptions` (apiUrl)
4.  **Set Admin Password:** Locate `Users__0__Password` and set a strong password.

### ⚠️ Important: Deploy Strategy
When saving your changes, look for the save bar at the bottom of the screen.
1.  Click the arrow icon next to the save button.
2.  Select **Save and deploy**.

> **Note:** Do not use "Save, rebuild, and deploy" unless you have changed the source code. "Save and deploy" is faster for configuration changes.

---

## Phase 3: Persistence Setup (Database)

By default, the blueprint uses `InMemory` storage (data is lost on restart). This step connects a real PostgreSQL database for persistence.

### 1. Choose and Create the Database

You have two main options for a managed PostgreSQL database:

* **Option A: Render PostgreSQL**
    1.  Go to the Render Dashboard and click **New +** -> **PostgreSQL**.
    2.  Create a database.
    3.  Once created, navigate to the database dashboard and view the **Connections** details.

* **Option B: Neon Serverless Postgres**
    1.  Create an account and a new project on **Neon.tech**.
    2.  Create a database and a new branch (usually `main`).
    3.  Navigate to the **Connection Details** and select the **.NET** or **Connection string** format.

### 2. Construct and Connect the Connection String

You need to provide the connection string in the DSN (Data Source Name) format.

1.  Return to your **TheIdServer** Web Service -> **Environment**.
2.  Update the following variables:
    * `DbType`: Change from `InMemory` to **`PostgreSQL`**.
    * `ConnectionStrings__DefaultConnection`: Set the value based on your choice:

| Option | Value for `ConnectionStrings__DefaultConnection` | Source |
| :--- | :--- | :--- |
| **Render** | `Host=[Hostname];Database=[Database];Username=[Username];Password=[Password];` | Retrieve these fields directly from the Render database **Connections** page. |
| **Neon** | Paste the **Connection string** provided by the Neon UI in the .NET tab. | Retrieve this string directly from the Neon UI. |

3.  Set the `Seed` variable to `"true"` for this initial deployment (to run migrations and create the Admin user).
4.  **Save:** Click the dropdown arrow and select **Save and deploy**.

### ✅ Result
The service will restart, apply database migrations, and seed the initial data. Your IdentityServer is now fully configured and persistent.
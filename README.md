
# Azure Quest Cloud Architecture Workshop
This is a sample project to be used during the demonstration of architecture techniques standards and best practices on the 2018 edition of the Azure Boot Camp event in Dublin, Ireland.

## Getting Started
**01.** Open the project and build it
**02.** Open console navigate to "**AzureQuest.Web**" folder and run the command  "`dotnet ef database update`"
 **03.** Open the database on the "**SQL Server Object Explorer**" and run the query "`SELECT * FROM dbo.AspnetUsers`"
 **04.** Run the application - register a new user - re-execute the sql query - check the results
 
## Securing the Web Application
**05.** Create a new account auth0.com 
**06.** Create a new project and name it `{AuthProjectName}`
**07.** Create an Application and name it "**`{AuthProjectName}` WebApp**" and select the type "*Regular Web Applications*"
**08.** Go to the settings page and set the following values:
> "Allowed Callback URLs": "https://localhost:44300/signin-auth0"
> "Allowed Web Origins": "https://localhost:44300"
> "Allowed Logout URLs": "https://localhost:44300"
> 
**09.** Open the file "`/AzureQuest.Web/appsettings.Development.json`" and setup *domain, clientId and clientSecret*
**10.** Open the file "`/AzureQuest.Web/Startup.cs`" un-comment the lines 27 and 28 *(or the Auth0 config lines)*
**11.** Right click on the project, set as startup project, run the application - register as new user - get authenticated
**12.** Check the file "`IdentityConfigurationService.cs`" to understand how the OAuth service is registered

## Securing the Web Api
**13.** Right click the solution and set the properties to start both projects, web and api
**14.** Go to auth0 and create an API and name it "**`{AuthProjectName}` WebApi**" and the Identifier as "`azure.quest.api`"
**15.** Go to the settings tab and check the general settings of the api client
**16.** Open the file "`/AzureQuest.Api/Startup.cs`" and set lines 59 and 60 *(or auth0 data)* with your client data
**17.** Go to the tab "**Machine to Machine Applications**" and authorise the "**`{AuthProjectName}` WebApp**"
**18.** Click on the "**Test**" tab and select **C#** on the next screen
**19.** Open the file "`/AzureQuest.Web/appsettings.json`" and set the property "**AzureQuestAPIAuth**" using the  C# testing code of the api on the auth0 page
**20.** After running the solution the "*Task Management*" menu should be appearing on the web application header
> You can check the `BaseController.cs` and the `_Layout.cshtml` to understand how this is happening.

## CosmosDB Setup
**21.** Go to Azure and create a new **CosmosDB** service
**22.** Create a new resource group as well for the new environment and set it to be `geo-redundant`
**23.** Go to the database resource page and create two collections "`Tasks`" and "`Notifications`" on a single database "`AzureQuest`"  according to the file "`/AzureQuest.Api/Providers/CosmosProvider.cs`"
**24.** While the creation is on going, understand what are resource groups on https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-overview
**25.** Open file "`/AzureQuest.Api/appsettings.json`" and set the database endpoint (ex. `https://{host}:443/`) and the key that you can get on the "*Connection String*" menu of the azure db page
**26.** Run the application, create a new task, go back to azure, and check the document explorer to compare the results

## Deployment
**27.** Create both apps on azure (*web and api*) with `Application Insights` enabled
**28.** Create a sql server and database on azure and take note of the user and password
**29** Open the local database on the "SQL Server Object Explorer" on visual studio and create a new project from it with the name "**AzureQuest.Database**"
**30.** Open the settings of the new database project and change the target platform to "`Microsoft Azure SQL Database v12`"
**31.** Add you client IP address on the sql server firewall on azure and Publish the database project *(there is a new menu for it on the database page)*
**32.** While the publishing take time, check the database features on Azure (`security, performance and recovery`)
**33.** Open file "`/AzureQuest.Web/appsettings.json`" and copy oauth settings from the "`appsettings.development.json`" replacing the app address for the recent created web app, also use the same data to fill the setting "`AzureQuestAPIAuth`"
**34.** Publish the web application and after finished, click on the login menu and check the error page
**35.** Go to the web app on azure e select Application Settings, and create both connection strings: 
>"MyAPIAddress": "https://{my-api-name}.azurewebsites.net/"
>"DefaultConnection": "data source={server}.database.windows.net;initial catalog={dbname};user id={user};password={password};MultipleActiveResultSets=True"
>
**36.** Goto the web app url and click on login again to see the new error
**37.** Go to the Auth0 control panel and add the azure web app url on the `callback, origin and logout` settings
**38.** Go back to the application, and try to login again - it should work by now!
**39.** Go to visual studio and publish the api on the api service you previous created
**40.** Navigate to the url `https://{MyApiName}.azurewebsites.net/swagger/`
**41.** Go back to the web app, refresh the page and check that the task manager menu is visible

## Notification Module
**42.** Go Azure and create a SendGrid service 
**43.** Access the service and go to manage - generate a new key - save the key
**44.** Go to azure active directory and register the you api as application
**45.** Click in the "Settings" menu and go to the tab "Keys", add a new password, set a description, expiration and value for it. After saving the page, a secret code will be generated for this key, this code will be displayed on top of the `value` field, copy this code, it will be your api secret key.
**46.** Access the "Required Permissions" tab and add permission to azure key vault, select all options
**47.** Go to azure and create a **Key Vault** service adding permission access to the registered api
**48.** Create a new secret on the vault with the following:
>"appSettings--apiKeys--sendGrid" : "{The key you generated on the SendGrid management panel}"

**49.** Open the file "`/AzureQuest.Api/azurekeyvault.json`" and set the `vault name` (strait forward), `api key` (on the active directory registration for the api, is the field "*Application ID*" and `secret` you generated and saved for the api.
**50.** Open the file "`/AzureQuest.Api/Program.cs`" un-comment line 18, or the call to "`VaultConfiguration`"
**51.** Go to SendGrid (access the service on azure and click on the menu "*Manage*") access the page `Templates/Transactional` and create a new template using the sample file "`/email.template.txt`"
**52.** Copy the template "`ID`" to the file "`/AzureQuest.Api/Repositories/NotificationRepository.cs`" line 47 replacing the placeholder "`{MyTransactionalTemplateId}`"
**53.** Go to Azure and create a storage account, while its creating check more about the service in  https://docs.microsoft.com/en-us/azure/storage/common/storage-introduction
**54.** Create a queue container and name it "`azurequest-notification-queue`"
**55.** Access the page "`Access keys`" of the new storage container and save the connection string of "`key1`"
**56.** Go back to the key vault and create a new secret with the following:
>"appSettings--apiKeys--azureQuestStorage" : "{storage container key1 connection string}"

**57.** Go to the file "`/AzureQuest.Api/Repositories/NotificationRepository.cs`" and check how this key is retrieved on line 184, by using ":" instead "--" on the navigation property
**58.** Publish the API, open the file "`/AzureQuest.Web/Views/Task/Details.cshtml`" and un-comment the options  `Cancel/Complete` then publish the web module as well
**59.** Create a new task, go to the details page and change its status to Complete or Cancel
**60.** Go back to azure and check the storage queue for the new notification item on the list

## Azure Function
**61.** Create a new Function App service on Azure selecting the already existent resources and storage
**62.** Create a new azure function project on visual studio and name it "`AzureQuest.NotificationFn`"
**63.** Select queue trigger and set the queue name correctly to identify your previous created queue
**64.** Change the name of the file "Function1.cs" to "NotificationQueueParser.cs"
**65.** Copy the function sample from the file "`/notification.function.txt`", make sure to preserve the correct queue name on top the function
**66.** Add reference to the common project and to the nuget package for RestSharp and fix the imports
**67.** Open the file "`/AzureQuest.NotificationFn/local.settings.json`" and add the keys to able to debugger locally the function:
>"AzureQuestAPIUrl": "http://localhost:54572",
>"AzureQuestAPIAuth": "{\"client_id\":\"{Auth0WebAppClientId}\",\"client_secret\":\"{Auth0WebAppClientSecret}\",\"audience\":\"{Auth0WebApiIdentifier}\",\"grant_type\":\"client_credentials\"}"
>
>(You can copy the *AzureQuestAPIAuth* value from the web app project)

**68.** Set a break point on the function and run it, if you have items on the queue it should hit
**69.** Go to the function on azure, in the application settings set the following values:
>"AzureQuestAPIUrl": "http://{MyApiName}.azurewebsites.net/"
>"AzureQuestAPIAuth": "{"client_id":"{Auth0WebAppClientId}","client_secret":"{Auth0WebAppClientSecret}","audience":"{Auth0WebApiIdentifier}","grant_type":"client_credentials"}"
>
>(You can copy the *AzureQuestAPIAuth*from the  curl section of the auth0 testing page, note that there are no backslashes in this value, different from the *local.settings.json* file)

**70.** Publish the function on the previous created function app service
**71.** Go to the task website and create another task, go to the details page and change it status to "`Complete/Cancel`" to generate another notification
**72.** Go to the function on azure and look at the page "Monitor" to see the processing of the notification
**73.** After the processing, go to the sendgrid control panel and access the activity menu to see the data of notification message
**74.** After you see the "Delivered" type of activity on sendgrid the email message should be in your inbox
**75.** Open the message on your inbox and go back to sendgrid to see the activity type "Open" in the list
>You can check the file "`/AzureQuest.Api/Providers/SendGridProvider.cs`" to understand how the integration is being handled and the tracking options are set

## Server Side Monitoring
**76.** Go to azure and select the AppInsigths created for our Api service, go to properties and get the instrumentation key of the service
**77.** Open file "`/AzureQuest.Api/appsettings.json`" and set the insights instrumentation key
**78.** Open file "`/AzureQuest.Api/Program.cs`" and un-comment the line 16 or the "`.UseApplicationInsights()`" call
**79.** Open file "/AzureQuest.Api/Startup.cs" and un-comment the line 61 or the "`services.AddApplicationInsightsTelemetry();`" call
**80.** Publish the api, navigate to the web app, access the task management menu and go back to the app insights on azure to check the data being collected
**81.** You can also try to use the swagger interface of the api to try some actions without authorisation and check the error details on the app insights page (`https://{YouApiName}.azurewebsites.net/swagger/`)
**82.** The same strategy can also be used to enable app insights on the other projects (web, function, etc) and benefit from the same functionality
**83.** Check other telemetry options the app insights page

## Server Side Monitoring
**84.** Go to https://sentry.io/ and create an account, go to your new project page, click on the settings tab and on the menu "`Error Tracking`"
**85.** Add the nuget package "SharpRaven" on the webproject (if not there yet)
**86.** Open the file "`/AzureQuest.Web/Services/CustomExceptionHandlerMiddleware.cs`" and un-comment the `RavenClient` lines updating the client key from the auth0 settings pages "`Popular/C#`"
**87.** Open the file "/AzureQuest.Web/Startup.cs" and un-comment the line 49 where the custom middleware is configured (if not un-commented already)
**88.** Open the file "`/AzureQuest.Web/Views/_Layout.cshtml`" and un-comment the sentry code at the bottom replacing the key from the auth0 settings page "`Popular/Javascript`"
**89.** Open the file "`/AzureQuest.Web/Views/Home/About.cshtml`" and un-comment the javascript code at the bottom that generates an error for testing purpose
**90.** Publish the application, make sure you logged out, and try to access the about menu, go back and authenticate yourself, now access the about page again. You can now go sentry dashboard and see the insights.

## Contributing
Contributions are welcome, just get in touch with me and we can sort it out.

## Authors
At this moment, only one warrior on this quest:

 - **Wagner Alves** - Solutions Architect - Dublin/Ireland
 - keyrox@gmail.com | https://www.linkedin.com/in/keyrox | https://github.com/keyrox

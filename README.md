# Setting up CBEP-Slack integration

## Create a Slack Webhook:
1\.  Browse to Slack apps.  Search for "Incoming webhooks"

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/webhook/1.png "Slack apps")

2\.  Click "Add Configuration"

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/webhook/2.png "Add WebHooks configuration")

3\.  Select the channel you want to post to.

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/webhook/3.png "Select channel")

4\.  Click "Add Incoming Webhook Configuration"

5\.  Copy the webhook URL to a safe location for now.
  -  You can scroll down to the bottom of the page and customize the appearance and text of the webhook.
  
![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/webhook/4.png "Copy URL")
  

## Get your API token:
1\.  Log into your Carbon Black Enterprise Protection console, then hover over "Administration" and select "Login Accounts"

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/token/1.png "Navigate to Login Accounts")

2\.  Find your account or the service account you wish to use, then click the edit icon.

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/token/2.png "Edit desired account")

3\.  Find the checkbox at the bottom for "Show API Token", check it, and if necessary click "Generate" if a token is not already displayed.

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/token/3.png "Display account's API token")

4\.  Copy this token to a safe location for now.


## Edit and compile integration service:
1\.  Open the SLN file in the repo with VisualStudio.

2\.  Open Service1.cs and if necessary switch to code view (F7).

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/compile/1.png "Open Service1.cs")

3\.  Towards the top of Service1.cs there are three variables: hookurl, server, apitoken.
  -  Enter the slack webhook url that you copied before into the hookurl variable.
  -  Enter your CBEP server into server (including "https")
    -  Make sure you can reach your CBEP server in IE without certificate warnings - otherwise this application will throw errors.
  -  Enter your CBEP API token into the API token variable.
  
![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/compile/2.png "Set variables")
  
4\.  Compile the service executable by clicking "Build - Build Solution"

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/compile/3.png "Build solution")

5\.  The executable will build as [reporoot]\bin\Debug\CBEP-slack-integration.exe.

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/compile/4.png "Find executable")


## Install the service:
1\.  Copy the service executable onto the computer you wish to run it from.

2\.  Open an admin command prompt and cd into your .NET framework directory (normally C:\Windows\Microsoft.NET\Framework64\v4.0.30319)

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/install/1.png "cd into .NET framework directory")

3\.  Run the command "installutil.exe [directory of service executable]\CBEP-slack-integration.exe"

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/install/2.png "install the service")

4\.  Once installutil.exe has completed, open services.msc and start the "CBEP - slack integration" service.

![](https://downloads.semrauconsulting.com/cbep-slack-integration/readme-images/install/3.png "start the service")


## Congrats!  Your installation should now be completed.  You could probably test it by generating an approval request.  Within a minute or so, you should get a notification in Slack.
# Samples of utilising TransferWise API 

The goal of this exercise was to set up a webhook that TransferWise would use to post data every time money gets deposited in our TransferWise account. Data provided in the payload can later be matched against bank statements. TransferWiseClient project deals with retrieval of account balances and bank statements.
Based on TransferWise documentation bank statement requests as well as webhook payloads are signed and verified with a pair of private/public keys to provide strong customer authentication.

### Includes 6 projects :

- TransferWiseClient to retrieve user profile id, account balance and bank statements
- TransferWiseCommon that provides strong customer authentication 
- TransferWiseWebhooks is to accept TransferWise payload which results from TransferWise balance credit event
- WebhookTest simulates TransferWise sending a balance credit payload. To be run together with TransferWiseWebhooks project
- 2 unit test projects to test TransferWiseCommon and TransferWiseWebhooks projects

### Requirements for integrating with TransferWise API

- Developer account with TransferWise https://sandbox.transferwise.tech/register
- TransferWise API access token with full access
- Private and public key pair for signing data
- Upload .pem file with public key to the TransferWise account
- Save .pem files with private and public keys to TransferWiseCommon/Certificates folder
- Create user secret that holds TransferWise API access token for TransferWiseClient project for local dev purposes
- Register for balance#credit event Webhook


### Technologies used

- .NET Core
- xUnit
- OpenSSL
- Bash

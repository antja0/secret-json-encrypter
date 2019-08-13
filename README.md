# secret-json-encrypter

Small utility originally developed for encrypting ASP NET secrets.

.NET Console application. Runs and asks user for parameters.

## Instructions

Run.

1. Asks user for RSA public key. If none given, generates a new pair and displays it on the console.
2. Asks user for `secret.json` file path.

## Functionality

Takes in secret.json formatted as 

```json
{
  "secretKey": "secretValue",
  "secretKey": "secretValue",
  "secretKey": "secretValue"
}
```

and encrypts its values.
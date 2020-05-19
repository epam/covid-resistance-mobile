# Covi.Client

Implementation of the interaction with the backend is located in the separate project. To simplify the process of updating endpoints interfaces, this project uses **Microsoft.Rest.ClientRuntime** package and [**AutoRest**](https://github.com/Azure/autorest).

In the **Generation** folder of this project there are two files: [**Platform.json**](../src/Covi.Client/Generation/Platform.json) and [**generation_script.bash**](../src/Covi.Client/Generation/generation_script.bash). **Platform.json** is the [Swagger](https://swagger.io/docs/specification/2-0/what-is-swagger/) file, which should be generated regarding the current backend environment. To run code generation you should do the following:

1. Install **AutoRest** or ensure that it is installed (sudo may be required);

```bash
npm install -g autorest
```

2. Execute **generation_script.bash**. It will clear previous version of code and generate the one corresponding to the current **Platform.json** file. Also **generation_script.bash** file requires execute rights, if they are not set;
3. If script finishes without any errors then code generation was successful.

#### Remarks:

You should not put any non-generated files in the **Services/Platform** folder. After script run these files might be removed.

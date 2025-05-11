## ELEMockApi
This is a lightweight mock server designed for API simulation. Built on Jint (JavaScript interpreter for .NET), it requires basic JavaScript knowledge to write rules and placeholders and uses SQLite as database.

> For calling your Api prepend your request Url with **mocked** for example  localhost:xx/originalapi/get =>  localhost:xx/mocked/originalapi/get

### Creating a Mock API Endpoint
To set up a mocked API, you must define the following components:

- HTTP Method 

- URL 

- Rule 

- Responses

### Http Methods:
**Supported HTTP Methods:**
- Get
- Post 
- Put
- Patch 
- Delete.

### Url :
###### The URL must contain only the base URL, excluding any query parameters.

- Ensure the endpoint does not include ? or query strings (e.g., key=value).

- Examples:

    - ✅ Valid:  https://api.example.com/data

    - ❌ Invalid: https://api.example.com/data?param=123

### Rule :

###### You must include a JavaScript function named evaluate that returns the appropriate HTTP status code (e.g., 200 (OK), 404 (Not Found), etc.) in the user interface panel.

- its returned status code must be explicitly declared in the response.

- This function can also be used to validate the request body, headers, or other parameters and return the corresponding response.

###### Within the rule context, you can access:

- The request body (if it exists): **req.body.YourPropertyName**

- The request headers: **req.headers**

- The query strings (if they exist): **req.queries**



###### Logging with console.log() in ELEMockApi 

You can use ```console.log()``` to:

1. Write logs to the database

    - All console output is automatically captured and stored

2. View logs in the ELEMockApi Panel
    - Access the complete logging history through the admin interface
ELEMockApi Panel**

**sample :**
```js
function evaluate(){

    //log request body in app logs
    console.log(JSON.stringify(req.body));

    //access to headers
    console.log(req.headers.Accept?.[0]);
    
    //access to query parameters
    console.log(req.queries?.age?.[0]);
    
    if(typeof req.body.age === 'undefined')
        return 400;

    if(req.body.age > 20)
        return 200;
    
    return 500;

    //you must declare all posible status codes in responses (400,200 and 500)
}
```

### Responses :
###### Define your response body and the corresponding HTTP status code (in a panel with a user interface).

- Responses must comply with the status codes defined in the rules section.

- In the response body, you can use placeholders that reference context values (such as req.body, req.headers, or req.query), which will be replaced with actual values at runtime.

- Additionally, you can include JavaScript code within placeholders (for example, to generate dynamic values like timestamps).


**For example :**
```js
//Syntax :[$YourContextValue$]
//Syntax :[$=>YourJsCode$]
{
    "result": {
        "nickName" : "ELE", //static value is response
        "age": "[$req.body.age$]",//this will replaced by age from request body
        "date": "[$=>new Date()$]" // this will filled by date time
    }
}
```













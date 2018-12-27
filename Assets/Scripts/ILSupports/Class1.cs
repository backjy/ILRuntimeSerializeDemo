using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ILRedirections
{
    static Dictionary<string, CLRRedirectionDelegate> redirections = new Dictionary<string, CLRRedirectionDelegate>
    {
        //{ "AddComponent<T>()",      AddComponent},
        //{ "AddComponent(Type)",     AddComponent_Type},
        //{ "AddComponent(String)",   AddComponent_string},
        //{ "GetComponent<T>()",      GetComponent},
        //{ "GetComponent(Type)",     GetComponent_Type},
        //{ "GetComponent(String)",   GetComponent_string},
    };

    static void Initialize()
    {

    }
}
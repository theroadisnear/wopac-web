using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace library_prototype.CustomClass
{
    public class CustomValidationMessage
    {
        public static List<string> GetErrorList(ModelStateDictionary modelState)
        {
            var errorList = from state in modelState.Values
                            from error in state.Errors
                            select error.ErrorMessage;
            return errorList.ToList(); 
        }
    }
}
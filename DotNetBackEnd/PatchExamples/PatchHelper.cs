using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMgmt.PatchExamples
{
    public class PatchHelper
    {
        public static bool TryApplyPatch<T>(JsonPatchDocument<T> patchDoc, T entity, ModelStateDictionary modelState)
            where T : class
        {
            if (patchDoc == null || entity == null)
                return false;

            patchDoc.ApplyTo(entity, modelState);
            return modelState.IsValid;
        }
    }
}

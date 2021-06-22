using System.Linq;

namespace AgolPlugin.Validation.Acad
{
    public class LayerNameValidator
    {
        public static readonly char[] InvalidLayerChars = { '<', '>', '/', '\\', '“', ':', ';', '?', '*', '|', '=', '‘' };

        public LayerNameValidator() { }

        public bool Validate(string str, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Any(c => InvalidLayerChars.Contains(c)))
            {
                errorMessage = $"Layer name cannot contain any of the following special characters: {string.Join(", ", InvalidLayerChars)}";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }
        public static string RemoveInvalidLayerNameChars(string str)
        {
            return string.Concat(str.Where(c => !InvalidLayerChars.Contains(c)));
        }
    }
}
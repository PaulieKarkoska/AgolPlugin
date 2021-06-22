using System.Text.RegularExpressions;

namespace AgolPlugin.Validation.Acad
{
    public class BlockNameValidator
    {
        public BlockNameValidator() { }

        public bool Validate(string str, out string errorMessage)
        {
            var regex = new Regex("^[a-zA-Z0-1_$-]+$");
            if (regex.IsMatch(str) && !string.IsNullOrWhiteSpace(str))
            {
                errorMessage = null;
                return true;
            }
            else
            {
                errorMessage = $"Block name cannot be blank and should only contain the following special characters: $ _ -";
                return true;
            }
        }
    }
}
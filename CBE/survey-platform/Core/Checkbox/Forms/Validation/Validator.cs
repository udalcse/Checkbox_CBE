
namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Base validator class
    /// </summary>
    public abstract class Validator<T>
    {
        /// <summary>
        /// Get a language specific validation message.
        /// </summary>
        /// <param name="languageCode">Language code</param>
        /// <returns>Validation message</returns>
        public abstract string GetMessage(string languageCode);

        /// <summary>
        /// Validate the type of input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract bool Validate(T input);
    }
}

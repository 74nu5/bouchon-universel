namespace BouchonUniversel.Filters
{
    #region Usings

    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    /// <summary>The add header parameters attribute.</summary>
    internal sealed class AddHeaderParametersAttribute : ActionFilterAttribute
    {
        #region Champs

        /// <summary>The parameter name.</summary>
        private readonly string parameterName;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="AddHeaderParametersAttribute"/> class.</summary>
        /// <param name="parameterName">The parameter name.</param>
        public AddHeaderParametersAttribute(string parameterName) => this.parameterName = parameterName;

        #endregion

        #region Méthodes publiques

        /// <summary>Called before the action executes, after model binding is complete.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext"/>.</param>
        public override void OnActionExecuting(ActionExecutingContext context) =>
            context.ActionArguments[this.parameterName] = context.HttpContext.Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());

        #endregion
    }
}
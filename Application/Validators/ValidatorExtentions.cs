using FluentValidation;

namespace Application.Validators
{
    public static class ValidatorExtentions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().MinimumLength(6).WithMessage("Password must be minimum of 6 character")
                .Matches("[A-Z]").WithMessage("Password must contain 1 upper case letter")
                .Matches("[a-z]").WithMessage("Password must have at least 1 lower case ")
                .Matches("[0-9]").WithMessage("Password must contain a number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain non alpha numeric ");

            return options;
        }
    }
}
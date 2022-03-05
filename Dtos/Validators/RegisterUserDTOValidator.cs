using System.Linq;
using FluentValidation;
using LibApp.Data;

namespace LibApp.Dtos.Validators {
    public class RegisterUserDTOValidator : AbstractValidator<RegisterUserDTO> {
        public RegisterUserDTOValidator(ApplicationDbContext dbContext) {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
            RuleFor(x => x.Email).Custom((value, context) => {
                var emailInUse = dbContext.Users.Any(u => u.Email == value);
                if (emailInUse)
                    context.AddFailure("Email", "Given email address is already in use");
            });
        }

    }
}

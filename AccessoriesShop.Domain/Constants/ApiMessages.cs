using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Constants
{
    public class ApiMessages
    {
        public static class Auth
        {
            /// <summary>
            /// The login success
            /// </summary>
            public const string LoginSuccess = "Login successful";
            /// <summary>
            /// The login failed
            /// </summary>
            public const string LoginFailed = "Invalid email or password";
            /// <summary>
            /// The account locked
            /// </summary>
            public const string AccountLocked = "Account is locked. Try again in {0} minutes.";
            /// <summary>
            /// The account locked multiple attempts
            /// </summary>
            public const string AccountLockedMultipleAttempts = "Account locked due to multiple failed login attempts. Try again after {0} minutes.";
            /// <summary>
            /// The account inactive
            /// </summary>
            public const string AccountInactive = "Account is inactive. Please contact support.";
            /// <summary>
            /// The account not verified
            /// </summary>
            public const string AccountNotVerified = "Account is not verified. Please verify your email.";
            /// <summary>
            /// The unauthorized
            /// </summary>
            public const string Unauthorized = "Unauthorized access";
            /// <summary>
            /// The invalid credentials
            /// </summary>
            public const string InvalidCredentials = "Invalid credentials";
            /// <summary>
            /// The password changed success
            /// </summary>
            public const string PasswordChangedSuccess = "Password changed successfully";
            /// <summary>
            /// The current password incorrect
            /// </summary>
            public const string CurrentPasswordIncorrect = "Current password is incorrect";
            /// <summary>
            /// The password must be different
            /// </summary>
            public const string PasswordMustBeDifferent = "New password must be different from current password";
            /// <summary>
            /// The access denied
            /// </summary>
            public const string AccessDenied = "You do not have permission to view this user.";
            /// <summary>
            /// The existed email
            /// </summary>
            public const string ExistedEmail = "email already exists.";
            /// <summary>
            /// The existed phone
            /// </summary>
            public const string ExistedPhone = "phone number already exists.";
            /// <summary>
            /// The register success
            /// </summary>
            public const string RegisterSuccess = "Registration successful. Please check your email to verify your account.";

        }

        public static class ServerError
        {
            public const string General = "Internal server error. Please try again later.";
            public const string NotImplemented = "This feature is not implemented yet.";
            public const string ServiceUnavailable = "Service is currently unavailable. Please try again later.";
            public const string Timeout = "The request has timed out. Please try again.";
            public const string SystemBusy = "";
        }

        public static class Order
        {
            public const string NotFound = "Order not found.";
            public const string Created = "Order created successfully.";
            public const string Updated = "Order updated successfully.";
            public const string Deleted = "Order deleted successfully.";
        }

        public static class Payment
        {
            public const string NotFound = "Payment not found.";
            public const string Created = "Payment created successfully.";
            public const string Deleted = "Payment deleted successfully.";

            public const string LinkCreated = "Payment link created successfully.";
            public const string PaySuccess = "Payment successfully";
            public const string PayFailed = "Payment failed. Please try again.";
        }


    }
}

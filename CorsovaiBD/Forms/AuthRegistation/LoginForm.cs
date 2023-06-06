using System;
using Foundation;
using AppKit;

namespace CorsovaiBD
{
    public partial class LoginForm : NSViewController
    {
        public LoginForm (IntPtr handle) : base(handle)
        {
        }


        //partial void loginButton(NSObject sender)
        //{
        //    // Perform login logic here
        //    //var username = usernameField.StringValue;
        //    //var password = passwordField.StringValue;

        //    //if (username == "a" && password == "p")
        //    //{
        //    //    // Login successful

        //    //    var storyboard = this.Storyboard;
        //    //    var viewController = storyboard.InstantiateControllerWithIdentifier("ViewController") as NSViewController;
        //    //    if (viewController != null)
        //    //    {
        //    //        PresentViewControllerAsModalWindow(viewController);
        //    //        this.View.Window.Close();
        //    //        // Corrected line
        //    //    }

        //    //}
        //    //else
        //    //{
        //    //    // Login failed
        //    //    NSAlert.WithMessage("Login Failed", "Invalid username or password.", "OK", null, null).RunModal();
        //    //}
        //}
    }
}


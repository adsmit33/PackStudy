using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Net;
using System.Collections.Specialized;
using Android.Content;
using Newtonsoft.Json;

namespace PackStudy
{
    [Activity(Label = "UserRegistration")]
    public class UserRegistration : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Registration);

            // First name
            TextView txtFirstNamePrompt = (TextView)FindViewById(Resource.Id.txtFirstNamePrompt);
            // Last name
            TextView txtLastNamePrompt = (TextView)FindViewById(Resource.Id.txtLastNamePrompt);
            // Email
            TextView txtEmail = (TextView)FindViewById(Resource.Id.txtEmailPrompt);
            // Password
            TextView txtPasswordPrompt = (TextView)FindViewById(Resource.Id.txtPasswordPrompt);
            // Confirm Password
            TextView txtConfirmPasswordPrompt = (TextView)FindViewById(Resource.Id.txtConfirmPasswordPrompt);
            // Go to next step
            Button btnNext = (Button)FindViewById(Resource.Id.btnNext);
            // Create method for validation of all entered data
            btnNext.Click += btnNext_Click;
        }
        private void btnNext_Click(object sender, EventArgs args)
        {
            // Variables
            bool firstNameTest;
            bool lastNameTest;
            bool emailTest;
            bool AllTestsPassed = false;

            // Connect to inputed text
            EditText etFirstName = (EditText)FindViewById(Resource.Id.etFirstName);
            EditText etLastName = (EditText)FindViewById(Resource.Id.etLastName);
            EditText etEmail = (EditText)FindViewById(Resource.Id.etEmail);
            EditText etPassword = (EditText)FindViewById(Resource.Id.etPassword);
            EditText etConfirmPassword = (EditText)FindViewById(Resource.Id.etConfirmPassword);

            // Test to make sure first and last name are all chars
            firstNameTest = IsAllAlphabetic(etFirstName.Text);
            lastNameTest = IsAllAlphabetic(etLastName.Text);

            if (firstNameTest == true && lastNameTest == true)
            {
                // Do nothing move on to next test
                AllTestsPassed = true;
            }
            else
            {
                // Send toast for error
                Context context = ApplicationContext;
                string error = "First and last name cannot contain any numbers or special characters";
                Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                toast.Show();
            }

            // Test to make sure email is in correct format
            emailTest = IsValidEmail(etEmail.Text);

            if (emailTest == true)
            {
                // Do nothing go to next test
                AllTestsPassed = true;
            }
            else
            {
                // Send toast for error
                Context context = ApplicationContext;
                string error = "Please make sure email if in correct format ex. youremail@provider.com";
                Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                toast.Show();
            }
            // Test to make sure password is strong
            PasswordScore passwordStrengthScore = PasswordAdvisor.CheckStrength(etEmail.Text);
            switch (passwordStrengthScore)
            {
                case PasswordScore.VeryWeak:
                case PasswordScore.Weak:
                    // Send toast for error
                    Context context = ApplicationContext;
                    string error = "Please make sure password is at least 6 charcters, Uppercase and special charcter";
                    Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                    toast.Show();
                    break;
                case PasswordScore.Medium:
                case PasswordScore.Strong:
                case PasswordScore.VeryStrong:
                    // Password deemed strong enough, allow user to be added to database etc
                    AllTestsPassed = true;
                    break;
            }

            // Compare both feilds of inputed password to make sure they are correct
            bool checkIfEqual = String.Equals(etPassword.Text, etConfirmPassword.Text);
            if (checkIfEqual == true)
            {
                // Go to next test
                AllTestsPassed = true;
            }
            else
            {
                // Send toast for error
                Context context = ApplicationContext;
                string error = "Passwords do not match";
                Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                toast.Show();
            }


            // Check to make sure all feilds are filled out
            if (etFirstName.Length() != 0 && etLastName.Length() != 0 && etEmail.Length() != 0 && etPassword.Length() != 0 && etConfirmPassword.Length() != 0)
            {
                // Filed is not empty do your code here.
                AllTestsPassed = true;
            }
            else
            {
                // Send toast for error
                Context context = ApplicationContext;
                string error = "Please fill out all fields";
                Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                toast.Show();
            }

            // If all tests pass procced to next page
            if (AllTestsPassed == true)
            {
                //etFirstName
                WebClient client = new WebClient();
                Uri uri = new Uri("http://packstudy-com.stackstaging.com/registration.php");
                NameValueCollection parameter = new NameValueCollection();

                parameter.Add("FirstName", etFirstName.Text);
                parameter.Add("LastName", etLastName.Text);
                parameter.Add("Email", etEmail.Text);
                parameter.Add("Password", etPassword.Text);
                parameter.Add("PhoneNumber", "123456789");

                byte[] returnValue = client.UploadValues(uri, parameter);
                string r = Encoding.ASCII.GetString(returnValue);

                if (r == "Sucess")
                {
                    
                    uri = new Uri("http://packstudy-com.stackstaging.com/login.php");
                    parameter = new NameValueCollection();
                    parameter.Add("Email", etEmail.Text);
                    parameter.Add("Password", etPassword.Text);
                    returnValue = client.UploadValues(uri, parameter);
                    r = Encoding.ASCII.GetString(returnValue);

                    List<User> users = JsonConvert.DeserializeObject<List<User>>(r);
                    User CurrentUser = users[0];
                    //since the user name and password work, store it in the shared prefrences 
                    ISharedPreferences sharedPrefrences = GetSharedPreferences("MyData", FileCreationMode.Private);
                    var prefEditor = sharedPrefrences.Edit();
                    prefEditor.PutInt("id", CurrentUser.Id);
                    prefEditor.PutString("FirstName", CurrentUser.FirstName);
                    prefEditor.PutString("LastName", CurrentUser.LastName);
                    prefEditor.PutString("Email", CurrentUser.Email);
                    prefEditor.PutString("Password", CurrentUser.Password);
                    prefEditor.PutString("PhoneNumber", CurrentUser.PhoneNumber);
                    prefEditor.PutString("Username", CurrentUser.Username);

                    prefEditor.Commit();
                    // Go to next page
                    Intent activityIntent = new Intent(this, typeof(SelectCourses));
                    StartActivity(activityIntent);

                }
                else
                {
                    Context context = ApplicationContext;
                    string error = "Did not upload to database correctly";
                    Toast toast = Toast.MakeText(context, error, Android.Widget.ToastLength.Long);
                    toast.Show();
                }


             }

        }

        bool IsAllAlphabetic(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsLetter(c))
                    return false;
            }

            return true;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    
}
}
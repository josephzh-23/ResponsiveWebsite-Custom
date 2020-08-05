using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;

using System.Drawing;
using System.Net.Mail;
using System.Net;
using MySql.Data.MySqlClient;

namespace ResponsiveWebsite_Custom
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        static class Globals
        {
            // global int
            public static string emailbody;

            // global function
            public static string HelloWorld()
            {
                return "Hello World";
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btPassRec_Click(object sender, EventArgs e)
        {
            String CS = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(CS))
            {
                MySqlCommand cmd = new MySqlCommand("select * from users where Email='" + tbEmailId.Text + "'", con);
                con.Open();
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                //Note here we are creating a new table "forgot-password"
                // in MySql 
                if (dt.Rows.Count != 0)
                {
                    String myGUID = Guid.NewGuid().ToString();
                    int Uid = Convert.ToInt32(dt.Rows[0][0]);

                    //Grab the date as well
                    string date = Convert.ToString(dt.Rows[0][2]);
                    MySqlCommand cmd1 = new MySqlCommand("insert into ForgotPassRequests values('" + myGUID + "','" + Uid + "',curdate())", con);
                    cmd1.ExecuteNonQuery();

                    //send email
                    String ToEmailAddress = dt.Rows[0][3].ToString();
                    String Username = dt.Rows[0][1].ToString();
                    String EmailBody = "https://localhost:44313/RecoverPassword.aspx?Uid=" + myGUID
                        + "&date=" + Server.UrlEncode(date);
                    MailMessage PassRecMail = new MailMessage("josephzh23@hotmail.com", ToEmailAddress);
                    PassRecMail.Body = EmailBody;
                    PassRecMail.IsBodyHtml = true;
                    PassRecMail.Subject = "Reset Password";

                    //Assign EmailBody to global variable
                    Globals.emailbody = EmailBody;

                    //Create a label for showing the reset link
                    Button1.Text = "click me to be taken to the password_recover page";

                    //SmtpClient SMTP = new SmtpClient("smtp-mail.outlook.com", 587);
                    SmtpClient SMTP = new SmtpClient("josephzh23@hotmail.com", 465);
                    SMTP.Credentials = new NetworkCredential()
                    {
                        UserName = "josephzh23@hotmail.com",
                        Password = "030801Job"
                    };
                    SMTP.EnableSsl = true;
                    //SMTP.Send(PassRecMail);

                    lblPassRec.Text = "Check your email to reset your password.";
                    lblPassRec.ForeColor = Color.Green;

                }
                else
                {
                    lblPassRec.Text = "OOps This email id DOES NOT exist in our database !";
                    lblPassRec.ForeColor = Color.Red;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.emailbody);
        }
    }
}
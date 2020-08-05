using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace ResponsiveWebsite_Custom
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // We grab the user info from cookies
            // And then put them in the textbox 
            if (!IsPostBack)
            {
                if (Request.Cookies["UNAME"] != null && Request.Cookies["PWD"] != null)
                {
                    UserName.Text = Request.Cookies["UNAME"].Value;
                    Password.Attributes["value"] = Request.Cookies["PWD"].Value;
                    CheckBox1.Checked = true;
                }
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String CS = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(CS))
            {
                // We need to build a new table 
                MySqlCommand cmd = new MySqlCommand("select * from Users where Username='" + UserName.Text + "' and Password='" + Password.Text + "' and Email='" + Email.Text + "'", con);
                con.Open();
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);


                // This checks if the MySql query ran successfully
                // and datatable is filled 
                if (dt.Rows.Count != 0)
                {
                    Session["USERNAME"] = UserName.Text;
                    Session["email"] = Email.Text;
                    Response.Redirect("~/UserHome.aspx");


                    if (CheckBox1.Checked)
                    {
                        Response.Cookies["UNAME"].Value = UserName.Text;
                        Response.Cookies["PWD"].Value = Email.Text;

                        Response.Cookies["UNAME"].Expires = DateTime.Now.AddDays(15);
                        Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(15);


                    }
                    else
                    {
                        Response.Cookies["UNAME"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(-1);
                    }


                    // This fetches user id from database 

                    string Utype;
                    Utype = dt.Rows[0][5].ToString().Trim();

                    if (Utype == "U")
                    {
                        Session["USERNAME"] = UserName.Text;
                        Response.Redirect("~/UserHome.aspx");
                    }
                    if (Utype == "A"){
                        Session["USERNAME"] = UserName.Text;
                        Response.Redirect("~/AdminHome.aspx");
                    }
                }
                else
                {
                    lblError.Text = "Invalid Username or Password !";
                }


            }
        }
    }
}
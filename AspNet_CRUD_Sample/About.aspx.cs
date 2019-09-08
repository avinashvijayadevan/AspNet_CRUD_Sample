using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNet_CRUD_Sample
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Instead of hardcoded values this can be retrieved from database
                GenderDropDown.DataSource = new List<string> { "Male", "Female" };
                GenderDropDown.DataBind();
                RefreshData();
            }
        }
        protected void GenderDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData(GenderDropDown.SelectedItem.Text);
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            RefreshData();
            //GridGenderDropDown.DataSource = new List<string> { "Male", "Female" };
            DataBind();
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            RefreshData();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=LocalDb;Trusted_Connection=True;");
            int id = Convert.ToInt16(GridView1.DataKeys[e.RowIndex].Values["cohort_id"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM [TASK2].[RISK_tb] WHERE [cohort_id] =@id", con);
            cmd.Parameters.AddWithValue("id", id);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            RefreshData();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=LocalDb;Trusted_Connection=True;");

            TextBox txtDiagnosis = GridView1.Rows[e.RowIndex].FindControl("TextBox1") as TextBox;
            TextBox txtAge = GridView1.Rows[e.RowIndex].FindControl("TextBox2") as TextBox;
            string txtGender = ((DropDownList)GridView1.Rows[e.RowIndex].FindControl("GridGenderDropDown")).SelectedItem.Text;
            string id = GridView1.DataKeys[e.RowIndex].Values["cohort_id"].ToString();

            con.Open();
            SqlCommand cmd = new SqlCommand("[TASK2].[sp_Capture_Risk_INSERT_UPDATE]", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("acs_diagnosis", txtDiagnosis.Text);
            cmd.Parameters.AddWithValue("age", txtAge.Text);
            cmd.Parameters.AddWithValue("gender", txtGender);
            cmd.Parameters.AddWithValue("cohort_id", id);

            int i = cmd.ExecuteNonQuery();
            con.Close();
            GridView1.EditIndex = -1;
            RefreshData();

        }

        public void RefreshData(string gender = null)
        {
            string selectQuery = "SELECT [cohort_id], [acs_diagnosis], [age], [gender] FROM [TASK2].[RISK_tb]";
            if (gender != null) { selectQuery = string.Format("SELECT [cohort_id], [acs_diagnosis], [age], [gender] FROM [TASK2].[RISK_tb] WHERE [gender] = {0}", "'" + gender + "'"); }
            SqlConnection con = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=LocalDb;Trusted_Connection=True;");
            SqlCommand cmd = new SqlCommand(selectQuery, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected IEnumerable<string> PopulateGridGenderDropDown()
        {
            return new List<string> { "Male", "Female" };
        }
    }
}
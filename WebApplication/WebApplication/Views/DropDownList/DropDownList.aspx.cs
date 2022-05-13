using System;
using System.Web.UI.WebControls;

namespace WebApplication.Views.DropDownList
{
    public partial class DropDownList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            getDropdownList(ddlb);
        }

        /// <summary>
        /// dropdownlist 설정
        /// </summary>
        /// <param name="ddlb"></param>
        private void getDropdownList(System.Web.UI.WebControls.DropDownList ddlb)
        {
            int now = DateTime.Now.Year;
            
            for (int i = now - 10; i <= now; i++)
            {
                ddlb.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

        }
        
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PBALBS
{
    public partial class AddEditBlock : System.Web.UI.Page
    {
        public int BlockId
        {
            get { return Convert.ToInt32(ViewState["BlockId"].ToString()); }
            set { ViewState["BlockId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                BlockId = 0;
                LoadDistrict();
                LoadBlockList();
            }
        }

        protected void LoadDistrict()
        {
            BusinessLayer.District ObjDistrict = new BusinessLayer.District();
            DataTable dt = ObjDistrict.GetAll();
            ViewState["District"] = dt;
        }

        protected void LoadBlockList()
        {
            BusinessLayer.Block ObjBlock = new BusinessLayer.Block();
            DataTable dt = ObjBlock.GetAll();
            DataRow dr = dt.NewRow();
            dr["BlockId"] = "0";
            dr["DistrictId"] = "0";
            dr["BlockName"] = "";
            dt.Rows.InsertAt(dr, 0);

            dgvBlock.DataSource = dt;
            dgvBlock.DataBind();
        }

        protected void dgvBlock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int BlockId = Convert.ToInt32(dgvBlock.DataKeys[e.Row.RowIndex].Values["BlockId"].ToString());
                int DistrictId = Convert.ToInt32(dgvBlock.DataKeys[e.Row.RowIndex].Values["DistrictId"].ToString());
                ((LinkButton)e.Row.FindControl("lnkUpdate")).CommandArgument = e.Row.RowIndex.ToString();

                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlDistrict");
                ddl.DataSource = (DataTable)ViewState["District"];
                ddl.DataBind();

                if (BlockId == 0)
                {
                    ((LinkButton)e.Row.FindControl("lnkUpdate")).Text = "Add";
                    ((CheckBox)e.Row.FindControl("ChkSelect")).Visible = false;
                    
                }
                else
                {
                    ((LinkButton)e.Row.FindControl("lnkUpdate")).Text = "Update";
                    ddl.SelectedValue = DistrictId.ToString();
                }
            }
        }

        protected void dgvBlock_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            BlockId = Convert.ToInt32(dgvBlock.DataKeys[e.RowIndex].Values["BlockId"].ToString());
            BusinessLayer.Block ObjBlock = new BusinessLayer.Block();
            ObjBlock.Delete(BlockId);
            LoadBlockList();
          
        }

        protected void dgvBlock_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("AddUpdate"))
            {
                int RowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                BlockId = Convert.ToInt32(dgvBlock.DataKeys[RowIndex].Values["BlockId"].ToString());

                BusinessLayer.Block ObjBlock = new BusinessLayer.Block();
                Entity.Block Block = new Entity.Block();
                Block.BlockId = BlockId;
                Block.DistrictId = int.Parse(((DropDownList)dgvBlock.Rows[RowIndex].FindControl("ddlDistrict")).SelectedValue);
                Block.BlockName = ((TextBox)dgvBlock.Rows[RowIndex].FindControl("txtBlock")).Text;
                ObjBlock.Save(Block);
                LoadBlockList();

            }
        }

        protected void btnExchange_Click(object sender, EventArgs e)
        {
            int[] arr = new int[2];
            int i = 0;
            foreach (GridViewRow row in dgvBlock.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chk = (CheckBox)row.FindControl("ChkSelect");
                    if (chk.Checked == true)
                    {
                        arr[i] = int.Parse(dgvBlock.DataKeys[row.RowIndex].Value.ToString());
                        i += 1;
                    }

                }
            }

            BusinessLayer.Block ObjBlock = new BusinessLayer.Block();
            ObjBlock.Exchange(arr[0], arr[1]);
            LoadBlockList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using productcommon;

namespace product.products
{

	public partial class Transferslip : Common.BasePage
    {
        protected StringBuilder result;
        protected string outwh = "", rpcode = "" ,inwh="";
        protected int curpage = 1, pagesize = 20, allCount = 0;
        protected SJYEntity.Common.Search sinfo = new SJYEntity.Common.Search();
        protected ProductBLL.Basebll.BaseList hb = new ProductBLL.Basebll.BaseList();
        //StringBuilder sqlpin = new StringBuilder();
        StringBuilder sqlCad = new StringBuilder();//查看编码是否重复
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (GetQueryString("inwh") != "")
                {
                    inwh = GetQueryString("inwh");
                }
                if (GetQueryString("rpcode") != "")
                {
                    rpcode = GetQueryString("rpcode");
                }
                if (GetQueryString("outwh") != "")
                {
                    outwh = GetQueryString("outwh");
                }

                try
                {
                    InitParam("");
                    bindData();
                }
                catch { }
            }
        }

        private void bindData()
        {
       DataTable rpcodedb = hb.getdate("rpcode,rpcode+'<>'+CAD AS rpname","dbo.products");

        
          

            result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < rpcodedb.Rows.Count; i++)
            {
                result.Append("{ value: \"" + rpcodedb.Rows[i]["rpcode"].ToString() + "\",label:\" " + rpcodedb.Rows[i]["rpname"].ToString() + "\"},");
            }
            result.Remove(result.Length - 1, 1);//把最后一个，给移除


            result.Append(" ]");



			string ispat = isPartcode();
            DataSet ds = getData();

            if (!ds.Equals(null))
            {
                allCount = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                lblCount.Text = allCount.ToString();

                DataTable dt = ds.Tables[1];
                dt.Columns.Add("iswh");//0,1,2



				DataTable dbstore = hb.getdate("SR_storecode", "dbo.SYS_RightStore WHERE SP_code='" + ispat + "'");
				for (int i = 0; i < dt.Rows.Count; i++) {


					if (dt.Rows[i]["states"].ToString() == "待出库") {
						DataRow[] dr = dbstore.Select("SR_storecode='" + dt.Rows[i]["outwh"].ToString() + "'");
					
					if (dr.Length >0) {

						dt.Rows[i]["iswh"]="0";//
					}
					}

					if (dt.Rows[i]["states"].ToString() == "待入库") {
						DataRow[] drr = dbstore.Select("SR_storecode='" + dt.Rows[i]["inwh"].ToString() + "'");

						if (drr.Length > 0) {

							dt.Rows[i]["iswh"] = "0";//
						}

					}
				}
			 


                dislist.DataSource = dt;
                dislist.DataBind();
                if (allCount <= pagesize)
                {
                    literalPagination.Visible = false;
                }
                else
                {
                    literalPagination.Visible = true;
					literalPagination.Text = GenPaginationBar("Transferslip.aspx?page=[page]&outwh=" + outwh + "&rpcode=" + rpcode + "&inwh=" + inwh + "", pagesize, curpage, allCount);
                } 
            }
        }


        private void InitParam(string Index)
        {

			string partcode = isPartcode();
            #region 
            sinfo.PageSize = 20;
            //sinfo.Tablename = "products"; 
           // sinfo.Orderby = "id";
            sinfo.Sqlstr = "(SELECT dbo.products.ShGoogcode,dbo.products.CAD,dbo.transferslip.id,dbo.transferslip.rpcode,QTY,states,rk.basename AS inWHName ,ck.basename AS outwhName,outwhtime,inwhtime,dbo.transferslip.opcode, outOpcode,inOpcode,dbo.transferslip.inserttime ,dbo.transferslip.inWH,dbo.transferslip.outwh,Dimension,PATTERN  FROM  transferslip  LEFT JOIN dbo.BaseStore rk ON  rk.Basecode=inWH   LEFT JOIN dbo.BaseStore ck ON  ck.Basecode=outwh  LEFT JOIN dbo.products ON dbo.products.rpcode=dbo.transferslip.rpcode )hh where ";


			sinfo.Sqlstr += "( EXISTS ( SELECT   SR_storecode FROM     dbo.SYS_RightStore rk  WHERE    inWH = rk.SR_storecode AND SP_code = '" + partcode + "' ) OR EXISTS ( SELECT    SR_storecode FROM      dbo.SYS_RightStore ck   WHERE     outwh = ck.SR_storecode AND SP_code = '" + partcode + "' ) )";

            if (rpcode != "")
            {
                sinfo.Sqlstr += "  and  rpcode  LIKE'%" + rpcode + "%' ";
            }
            else
            {
                sinfo.Sqlstr += " and 1=1 ";
            }
            if (outwh != "")
            {
				sinfo.Sqlstr += "  and outwhName  LIKE'%" + outwh + "%' ";
            }
            if (inwh != "")
            {
				sinfo.Sqlstr += "  and inWHName  LIKE'%" + inwh + "%' ";
            }

            if (Index == "")
            {
                #region

                if (!string.IsNullOrEmpty(GetQueryString("page")))
                {
                    curpage = int.Parse(GetQueryString("page"));
                    sinfo.PageIndex = curpage;
                }
                #endregion
            }
            else
            {

                sinfo.PageIndex = 1;
            }
            #endregion
        }



        /// <summary>
        /// 信息
        /// </summary>
        /// <returns></returns>
        private DataSet getData()
        {
            DataSet dt = new DataSet();
            dt = hb.QXGetprodList(sinfo);

            return dt;
        }
        protected void btnQuerey_Click(object sender, EventArgs e)
        {
            // panduan();
            if (!string.IsNullOrEmpty(Request.Form["inwh"]))
            {
                inwh = Request.Form["inwh"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Form["rpcode"]))
            {
                rpcode = Request.Form["rpcode"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Form["outwh"]))
            {
                outwh = Request.Form["outwh"].Trim();
            }
            try
            {
                InitParam("query");
                bindData();
            }
            catch { }
        }
       
    }
}
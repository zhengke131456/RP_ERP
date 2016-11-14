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

    public partial class proStore_Store : Common.BasePage
    {


        protected int curpage = 1, pagesize = 20, allCount = 0;
        protected string District = "", part;
        protected string cai = "", strockName = "", xinghao = "";
        protected SJYEntity.Common.Search sinfo = new SJYEntity.Common.Search();
        protected ProductBLL.Basebll.BaseList hb = new ProductBLL.Basebll.BaseList();
        protected StringBuilder result = new StringBuilder();
        protected StringBuilder sqlCad = new StringBuilder();//查看编码+年份是否重复
        protected int n = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            part = isPartcode(); //角色
            cai = Request.QueryString["QCAI"];
            strockName = Request.QueryString["strockName"];
            xinghao = Request.QueryString["xh"];
            if (!IsPostBack)
            {
                Hidpart.Value = isPartcode();
                try
                {
                    InitParam("");
                    bindData();
                    bind();
                }
                catch { }

            }
        }
        /// <summary>
        /// 绑定油站名称数据
        /// </summary>
        private void bind()
        {
            DataTable dt;

            // 根据仓库权限 来显示仓库
            //string sql = " (SELECT Basecode,basename FROM dbo.BaseStore INNER JOIN   dbo.SYS_RightStore  ON  Basecode=SR_storecode WHERE SP_code='" + Hidpart.Value + "')hh";
            string str = "(select * from BaseStore where Basecode in (select SR_storecode from SYS_RightStore where SP_code='" + Hidpart.Value + "'))hh";
            dt = hb.getdate(" * ", str);

            dpstoreNew.DataSource = dt;

            dpstoreNew.DataTextField = "basename";
            dpstoreNew.DataValueField = "basename";
            dpstoreNew.DataBind();
            dpstoreNew.Items.Insert(0, "请选择");

            //绑定全部型号列表
            str = "(select * from baseinfo where type=3)hh";
            dt = hb.getdate("*", str);

            dimension.DataSource = dt;

            dimension.DataTextField = "basename";
            dimension.DataValueField = "basename";
            dimension.DataBind();
            dimension.Items.Insert(0, "请选择");
        }

        private void bindData()
        {

            DataTable rpcodedb = hb.getdate("rpcode,rpcode+'<>'+CAD AS rpname", "dbo.products");





            result.Append("[");
            for (int i = 0; i < rpcodedb.Rows.Count; i++)
            {
                result.Append("{ value: \"" + rpcodedb.Rows[i]["rpcode"].ToString() + "\",label:\" " + rpcodedb.Rows[i]["rpname"].ToString() + "\"},");
            }
            result.Remove(result.Length - 1, 1);//把最后一个，给移除


            result.Append(" ]");


            DataSet ds = getData();

            if (!ds.Equals(null))
            {
                allCount = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                lblCount.Text = allCount.ToString();

                DataTable dt = ds.Tables[1];


                dislist.DataSource = ds.Tables[1];
                dislist.DataBind();
                if (allCount <= pagesize)
                {
                    literalPagination.Visible = false;
                }
                else
                {
                    literalPagination.Visible = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("proStore_Store.aspx?page=[page]");
                    if (strockName != "")
                        sb.Append("&strockName=" + strockName);
                    if (xinghao != "")
                        sb.Append("&xh=" + xinghao);
                    if (cai != "")
                        sb.Append("&QCAI=" + cai);
                    literalPagination.Text = GenPaginationBar(sb.ToString(), pagesize, curpage, allCount);
                    //"proStore_Store.aspx?page=[page]&QCAI='" + cai + "'&strockName='" + strockName + "'"
                }
            }
        }


        private void InitParam(string Index)
        {
            #region
            sinfo.PageSize = 20;

            sinfo.Orderby = "ID";

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
            #region 根据权限获取数据


            /*
            if (District == "")//系统管理员 区域权限
            {

				sinfo.Sqlstr = "(SELECT dbo.stock_storck.ID,dbo.stock_storck.rpcode,stockNum,basename as stockName,desNote ,stockID FROM dbo.stock_storck  LEFT JOIN dbo.BaseStore ON  dbo.BaseStore.id=stockID )hh ";
               

            }
            else
            {
				sinfo.Sqlstr = "(SELECT dbo.stock_storck.ID,dbo.stock_storck.rpcode,dbo.products.model,stockNum,basename as stockName,desNote  FROM dbo.stock_storck  LEFT JOIN dbo.baseinfo ON  dbo.baseinfo.id=stockID LEFT JOIN dbo.products  ON dbo.products.CAI=dbo.stock_storck.CAI WHERE [type]=4 AND  area='" + District + "'  )hh ";
                
            }
            if (cai!="" )
            {
                sinfo.Sqlstr += " where  CAI= '" + cai + "'";
            }
            else
            {
                sinfo.Sqlstr += " where 1=1 ";
            }

            if (strockName != "")
            {
                sinfo.Sqlstr += " and  stockName LIKE'%" + strockName + "%' ";
            }
		  */
            #endregion

            if (Index == "query")
            {
                sinfo.Sqlstr = "(SELECT dbo.stock_storck.ID,dbo.stock_storck.rpcode,stockNum,basename as stockName,Basecode,desNote ,stockID ,    ''''+ CONVERT(VARCHAR(30),ShGoogcode) as ShGoogcode,dbo.products.CAD,Dimension,PATTERN FROM dbo.stock_storck  LEFT JOIN dbo.BaseStore ON  dbo.BaseStore.id=stockID INNER JOIN dbo.SYS_RightStore ON stockcode=SR_storecode LEFT JOIN dbo.products ON dbo.products.rpcode=dbo.stock_storck.rpcode where sp_code='" + part + "')hh ";
            }
            else
            {
                sinfo.Sqlstr = "(SELECT dbo.stock_storck.ID,dbo.stock_storck.rpcode,stockNum,basename as stockName,Basecode,desNote ,stockID ,   ShGoogcode  ,dbo.products.CAD,Dimension,PATTERN FROM dbo.stock_storck  LEFT JOIN dbo.BaseStore ON  dbo.BaseStore.id=stockID INNER JOIN dbo.SYS_RightStore ON stockcode=SR_storecode LEFT JOIN dbo.products ON dbo.products.rpcode=dbo.stock_storck.rpcode where sp_code='" + part + "')hh ";
            }
            if (!string.IsNullOrEmpty(cai))
            {
                sinfo.Sqlstr += " where  rpcode= '" + cai + "'";
            }
            else
            {
                sinfo.Sqlstr += " where 1=1 ";
            }

            if (!string.IsNullOrEmpty(strockName))
            {
                sinfo.Sqlstr += " and  stockName LIKE'%" + strockName + "%' ";
                dpstoreNew.SelectedValue = strockName;
            }
            if (!string.IsNullOrEmpty(xinghao))
            {
                sinfo.Sqlstr += " and  Dimension ='" + xinghao + "' ";
                dimension.SelectedValue = xinghao;
            }
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            //DataTable ProductPrice = DataMgr.GetTableProPrice();
            //DataRow drproduct;
            //bool fileIsValid = false;
            //如果确认了文件上传，则判断文件类型是否符合要求

            #region MyRegion
            /*

            if (this.FileUpload1.HasFile)
            {
                //获取上传文件的后缀名
                String fileExtension = System.IO.Path.GetExtension(this.FileUpload1.FileName).ToLower();//ToLower是将Unicode字符的值转换成它的小写等效项

                //判断文件类型是否符合要求

                if (fileExtension == ".csv")
                {
                    fileIsValid = true;
                }
                else
                {
                    Response.Write("<script type='text/javascript'>window.parent.alert('文件格式不正确!请上传正确格式的文件')</script>");
                    return;
                }

            }
            //如果文件类型符合要求，则用SaveAs方法实现上传，并显示信息
            if (fileIsValid == true)
            {
               
                try
                {
                    string name = Server.MapPath("~/uploadxls/") + "ProPrice" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "") + ".csv";
                    this.FileUpload1.SaveAs(name);
                    if (File.Exists(name))
                    {

                       
                        DataTable dt = ProductBLL.Search.Searcher.OpenCSV(name);
                        if (dt.Rows.Count > 0)
                        {   
                            #region    先检测execl 是否存在数据重复，然后在和表对比
                           // 检测CAD(编码)编码列+年份 是否有重复 如果有重复就终止操作并提示重复记录
                            var CADPrice = from row in dt.Rows.Cast<DataRow>()

                                           group row by new { CP_CAD = row["CAD"].ToString(), CP_Yeat = row["year"].ToString(), } into result
                                 select new { Peo = result.Key, Count = result.Count() };

                            foreach (var group in CADPrice)
                            {
                                if (Convert.ToInt32(group.Count) > 1)
                                {
                                    sqlCad.Append( group.Peo.CP_CAD + "年份:" + group.Peo.CP_Yeat + ",");
                                }
                            }





                            if (sqlCad.Length == 0)//没有重复项
                            {
                                string str = "PRSP_CAD,PRSP_Year";
                                string table = "ProductPrice";
                                DataTable dbProductPrice = hb.getdate(str, table);//数据库表
                                
                                string tableinfo = "baseinfo where type=1";
                                DataTable dbbaseinfo = hb.getdate("ID,basename", tableinfo);//返回编码

                                #region 和数据库表对比
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {

                                    string ss = "PRSP_CAD='" + dt.Rows[i]["CAD"].ToString() +"' and PRSP_Year='" + dt.Rows[i]["year"].ToString()+"'";
                                    DataRow[] sRow = dbProductPrice.Select(ss);


                                    if (sRow.Length != 0)//和ProductPrice 数据对比
                                    {
                                        sqlCad.Append(dt.Rows[i]["CAD"].ToString() + "年份:" + dt.Rows[i]["year"].ToString() + ",");
                                    }
                                    else
                                    {
                                        DataRow[] RowID = dbbaseinfo.Select("basename='" + dt.Rows[i]["CAD"].ToString() + "'");

                                        if (RowID.Count() == 0)
                                        {
                                            sqlCad.Append(dt.Rows[i]["CAD"].ToString() + ",");
                                            n = 1;
                                        }
                                        else
                                        {
                                            drproduct = ProductPrice.NewRow();
                                            drproduct[0] = dt.Rows[i]["CAD"].ToString();
                                            drproduct[1] = dt.Rows[i]["WHSprice"].ToString();
                                            drproduct[2] = Convert.ToDecimal(dt.Rows[i]["Inprice"].ToString());
                                            drproduct[3] = dt.Rows[i]["year"].ToString();
                                            drproduct[4] = RowID[0]["ID"].ToString();//编码ID

                                            ProductPrice.Rows.Add(drproduct);
                                        }
                                    }
                                }
                                #endregion

                                #region  执行插入

                                if (sqlCad.Length == 0)//如果没有重复项
                                {

                                    //if (hb.insetpro(sqlpin.ToString()))
                                    if (DataMgr.BulkToDBProPrice(ProductPrice))
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传成功');window.location.href='../products/productPrice.aspx';</script>");
                                        ProductPrice.Clear();
                                        return;
                                    }
                                    else
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传失败');</script>");
                                        return;
                                    }

                                }
                                else
                                {
                                    if (n == 0)
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传失败！上传数据和数据库存在重复：" + sqlCad.ToString() + "');</script>");
                                    }
                                    else
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传失败！请在编码列表处维护编码,新编码：" + sqlCad.ToString() + "');</script>");
                                    }
                                   

                                    sqlCad.Clear();
                                    ProductPrice.Clear();
                                    return;
                                }

                                #endregion
                            }
                            else
                            {
                               
                                    Response.Write("<script type='text/javascript'>window.parent.alert('上传失败！产品价格存在重复：" + sqlCad.ToString() + "');</script>");
                              
                                sqlCad.Clear();
                                ProductPrice.Clear();
                                return;
 
                            }

                           

                          
                            #endregion
                            
                        }
                        else
                        {
                            Response.Write("<script type='text/javascript'>window.parent.alert('文件无内容');</script>");
                            return;
                        }
                    }

                }
                catch ( Exception ex)
                {
                    ex.Message.ToString();
                    Response.Write("<script type='text/javascript'>window.parent.alert('文件内容格式不正确，请核实')</script>");
                    return;
                }
                finally
                {
                    ProductPrice.Dispose();
                }
            }
            */
            #endregion
        }
        protected void btnQuerey_Click(object sender, EventArgs e)
        {
            Querey();
            try
            {
                InitParam("query");
                bindData();
            }
            catch { }
        }


        public void Querey()
        {
            if (!string.IsNullOrEmpty(Request.Form["QCAI"]))
            {
                cai = Request.Form["QCAI"].Trim();
            }
            else cai = "";
            if (dpstoreNew.SelectedItem.ToString() != "请选择")
            {
                strockName = dpstoreNew.SelectedItem.ToString();
            }
            else strockName = "";
            if (dimension.SelectedItem.ToString() != "请选择")
            {
                xinghao = dimension.SelectedItem.ToString();
            }
            else xinghao = "";
        }

        protected void Buttondow_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("~/downloadxlsx/Order/");
            string datetime = "库存" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "");

            string detailfile = ".csv";//明细



            if (ProductBLL.download.ExcelHelper.TableToCsv(getTable(), path + datetime + detailfile))
            {
                downloadfile(path + datetime + detailfile);
                Response.Write("<script type='text/javascript'>window.parent.alert('导出EXecl成功!')</script>");

                return;
            }
            else
            {
                Response.Write("<script type='text/javascript'>window.parent.alert('导出Execl失败!');window.location.href='../products/PriceRebateExecl.aspx';</script>");

                return;
            }






        }
        protected DataTable getTable()
        {

            Querey();//获取查询参数
            InitParam("query");
            string strsql = "rpcode as '睿配编码',ShGoogcode  as '石化商品编码',cad as '供应商编码',Dimension as '型号',PATTERN as '花纹'";
            strsql += ",stockName  as '仓库名',	Basecode  as '仓库编码',stockNum as '数量'  ";


            DataTable dtdb = hb.getdate(strsql, sinfo.Sqlstr);

            return dtdb;
        }

        void downloadfile(string s_path)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(s_path);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();

            Response.AddHeader("Content-Disposition", "attachment;filename=" + file.Name);
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.WriteFile(file.FullName);
            Response.Flush();
            HttpContext.Current.Response.Clear();
            //下载完成后删除服务器下生成的文件
            if (File.Exists(s_path))
            {
                File.Delete(s_path);

            }
            HttpContext.Current.Response.End();
        }

    }


}
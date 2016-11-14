using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using System.Collections;
using System.Configuration;

namespace product.products
{
    public partial class outproductList : Common.BasePage
    {
        protected int curpage = 1, pagesize = 20, allCount = 0;
        protected string cai = "", Edate, Bdate;
        protected SJYEntity.Common.Search sinfo = new SJYEntity.Common.Search();
        protected ProductBLL.Basebll.BaseList hb = new ProductBLL.Basebll.BaseList();
        StringBuilder sqloutID = new StringBuilder();
        StringBuilder sqlspin = new StringBuilder();
        protected StringBuilder result = new StringBuilder();
        StringBuilder sqlsql = new StringBuilder(); //记录cai
        protected void Page_Load(object sender, EventArgs e)
        {
            Bdate = GetQueryString("bdate");
            Edate = GetQueryString("edate");
            cai = GetQueryString("q_cai");

            if (!IsPostBack)
            {
                //  hiddtype.Value = isDistrictRights();
                hiddpart.Value = isPartcode();

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
            DataTable rpcodedb = hb.getdate("rpcode,rpcode AS rpname", "dbo.products");
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

                dt.Columns.Add("markName");
                IDictionary dictmark = (IDictionary)ConfigurationManager.GetSection("spmark");
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    for (int j = 1; j <= dictmark.Count; j++)
                    {
                        string type = "type" + j;
                        if (dt.Rows[i]["spmark"].ToString() == j.ToString())
                        {
                            dt.Rows[i]["markName"] = dictmark[type].ToString();
                        }
                    }

                }

                dislist.DataSource = ds.Tables[1];
                dislist.DataBind();
                if (allCount <= pagesize)
                {
                    literalPagination.Visible = false;
                }
                else
                {
                    literalPagination.Visible = true;
                    literalPagination.Text = GenPaginationBar("outproductList.aspx?page=[page]&bdate=" + Bdate + "&edate=" + Edate + "&q_cai=" + cai + "", pagesize, curpage, allCount);
                }
            }
        }


        private void InitParam(string Index)
        {
            #region
            sinfo.PageSize = 20;
            //sinfo.Tablename = "inproduct";
            //sinfo.Orderby = "id";

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




            sinfo.Sqlstr = "  ( SELECT INbatch,dbo.outproduct.usercode ,dbo.outproduct.id ,dbo.outproduct.rpcode ,dbo.products.ShGoogcode,dbo.products.CAD , OD , QTY ,WHcode ,spmark , basename AS  basename ";
            sinfo.Sqlstr += " FROM dbo.outproduct INNER JOIN  dbo.SYS_RightStore ON  WHcode= SR_storecode     LEFT JOIN dbo.BaseStore  ON WHcode=Basecode  LEFT JOIN dbo.products ON dbo.outproduct.rpcode =dbo.products.rpcode ";
            sinfo.Sqlstr += "	WHERE SP_code='" + hiddpart.Value + "'  ";

            if (ispartRights() != "系统管理员")
            {
                sinfo.Sqlstr += " AND usercode='" + userCode() + "' ";
            }
            if (!string.IsNullOrEmpty(Bdate) && !string.IsNullOrEmpty(Edate))
            {
                sinfo.Sqlstr += " and   CONVERT(VARCHAR(20), OD, 23) BETWEEN '" + Bdate + "' AND  '" + Edate + "' ";
            }
            if (cai != "")
            {
                sinfo.Sqlstr += " and  outproduct.rpcode LIKE'%" + cai + "%' )hh ";
            }
            else
            {
                sinfo.Sqlstr += " )hh ";
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
            if (!string.IsNullOrEmpty(Request["QCAI"]))
            {
                cai = Request.Form["QCAI"].Trim();
            }
            else cai = "";
            if (!string.IsNullOrEmpty(Request["bdata"]))
            {
                Bdate = Request["bdata"].Trim();
            }
            else Bdate = "";
            if (!string.IsNullOrEmpty(Request["edata"]))
            {
                Edate = Request["edata"].Trim();
            }
            else Edate = "";
            try
            {
                InitParam("query");
                bindData();
            }
            catch { }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool fileIsValid = false;
            //如果确认了文件上传，则判断文件类型是否符合要求

            #region MyRegion


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
            #endregion

            //如果文件类型符合要求，则用SaveAs方法实现上传，并显示信息
            if (fileIsValid == true)
            {
                try
                {
                    string name = Server.MapPath("~/uploadxls/") + "OUT" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "") + ".csv";
                    this.FileUpload1.SaveAs(name);
                    if (File.Exists(name))
                    {
                        DataTable dt = ProductBLL.Search.Searcher.OpenCSV(name);
                        int qty = 0; int numsl = 0;
                        #region  exec 有数据

                        int s = 0;
                        {


                            string usercode = userCode();

                            #region 循环判断仓库权限及数量   2016-05-30改
                            List<products> list = new List<products>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string storeName = dt.Rows[i]["仓库编码"].ToString();
                                numsl = Convert.ToInt32(dt.Rows[i]["数量"].ToString());
                                string rpcode = dt.Rows[i]["睿配编码"].ToString();

                                if (list.AsEnumerable().Where(p => p.RpCode == rpcode).Where(p => p.StoreName == storeName).SingleOrDefault() != null)
                                {
                                    list.AsEnumerable().Where(p => p.RpCode == rpcode).Where(p => p.StoreName == storeName).Single().Number += numsl;
                                }
                                else
                                {
                                    list.Add(new products() { Number = numsl, RpCode = rpcode, StoreName = storeName });
                                }
                            }
                            DataTable dtstore = hb.getdate("SR_storecode", "dbo.SYS_RightStore WHERE  SP_code='" + hiddpart.Value + "'");  //可操作的仓库编码
                            foreach (products item in list)
                            {
                                DataRow[] dr = dtstore.Select("SR_storecode='" + item.StoreName + "'");
                                if (dr.Length <= 0)
                                {
                                    sqloutID.Append(item.StoreName + ",");
                                }
                                if (sqloutID.Length <= 0)
                                {
                                    qty = hb.getproScalar("SELECT stockNum FROM  dbo.stock_storck   where  stockcode='" + item.StoreName + "' AND rpcode='" + item.RpCode + "'");

                                    if (qty < item.Number)
                                    {
                                        sqlspin.Append( item.RpCode + " - " + item.StoreName + ',');
                                    }
                                    if (sqlspin.Length > 0)
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('出库数量大于库存数请核实" + sqlspin.ToString() + "');window.location.href='../products/outproductList.aspx';</script>");
                                        return;
                                    }
                                }
                                else
                                {
                                    Response.Write("<script type='text/javascript'>window.parent.alert('当前人员没有权限操作该仓库:" + sqloutID.ToString() + "');</script>");
                                    return;
                                }
                            }
                            #endregion
                            list.Clear();

                        #endregion

                            #region 批次号
                            //LT+R/C日期 +1
                            //当天导入 数字累加
                            //LTR20151120-1


                            int numbatch = 1;
                            string day = DateTime.Today.ToString("yyyy-MM-dd");
                            string Scalar = hb.GetScalarstring("  ISNULL(MAX(batchCount),'') ", " dbo.outproduct   where CONVERT(VARCHAR(20),insettime,23)='" + day + "'");

                            if (Scalar != "")
                            {
                                numbatch = Convert.ToInt32(Scalar) + 1;
                            }

                            #endregion

                            #region 执行 出库命令


                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string storeName = dt.Rows[i]["仓库编码"].ToString();
                                numsl = Convert.ToInt32(dt.Rows[i]["数量"].ToString());
                                string rpcode = dt.Rows[i]["睿配编码"].ToString();
                                #region 标识c处理
                                string tsbs = dt.Rows[i]["标识"].ToString().Trim();
                                if (tsbs != "")
                                {
                                    switch (tsbs)
                                    {
                                        case "正常入库":
                                            tsbs = "1";
                                            break;
                                        case "正常销售出库":
                                            tsbs = "2";
                                            break;
                                        case "退货入库":
                                            tsbs = "3";
                                            break;
                                        case "仓库置换":
                                            tsbs = "4";
                                            break;
                                        case "货品内销":
                                            tsbs = "5";
                                            break;
                                        case "特殊削减":
                                            tsbs = "6";
                                            break;
                                        case "特殊出库":
                                            tsbs = "7";
                                            break;
                                        case "调拨出库":
                                            tsbs = "8";
                                            break;
                                        default:
                                            tsbs = "1";
                                            break;
                                    }
                                }
                                if (tsbs == "6" || tsbs == "7")
                                {
                                    if (dt.Rows[i]["特殊说明"].ToString() == "")
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('标识为特殊入库或者特殊消减时特殊说明不能为空');window.location.href='../products/outproductList.aspx';</script>");
                                        return;
                                    }
                                }
                                if (tsbs == "8")
                                {

                                    Response.Write("<script type='text/javascript'>window.parent.alert('已取消调拨出库，请执行调拨');window.location.href='../products/outproductList.aspx';</script>");
                                    return;

                                }
                                #endregion


                                #region 执行出口操作 不用


                                //sqlsql.Append("INSERT  INTO dbo.outproduct ( rpcode, OD, WHcode, QTY, spmark,TSNote,usercode ) VALUES  ('" + rpcode + "', '" + dt.Rows[i]["日期"].ToString() + "', '" + storeName + "'," + numsl + "," + tsbs + ",'" + dt.Rows[i]["特殊说明"].ToString() + "','" + usercode + "')");

                                //sqlsql.Append("UPDATE dbo.stock_storck  SET stockNum=stockNum-" + numsl + " ,datechange=GETDATE() WHERE  rpcode='" + rpcode + "'AND  stockcode =" + storeName + "  ");

                                #endregion

                                string batch = rpcode.Substring(0, 2);
                                string batchnum = batch + "C" + day.Replace("-", "") + "-" + numbatch;


                                #region   执行出库 根据返回数字解析结果

                                string sqlstr = "exec pro_outstore  '" + rpcode + "', '" + dt.Rows[i]["日期"].ToString() + "', '" + storeName + "'," + numsl + "," + tsbs + ",'" + dt.Rows[i]["特殊说明"].ToString() + "','" + usercode + "','" + batchnum + "','" + numbatch + "' ";



                                if (!hb.ProExecinset(sqlstr))
                                {
                                    s = 1;
                                    break;
                                }
                                #endregion


                            }

                            #endregion



                            if (s == 0)
                            {

                                Response.Write("<script type='text/javascript'>window.parent.alert('出库成功');</script>");
                                return;
                            }
                            else
                            {
                                Response.Write("<script type='text/javascript'>window.parent.alert('出库失败请删除记录');</script>");
                                return;

                            }


                        }
                    }
                    else
                    {
                        Response.Write("<script type='text/javascript'>window.parent.alert('文件无内容');</script>");

                        return;
                    }


                }
                catch
                {
                    Response.Write("<script type='text/javascript'>window.parent.alert('文件内容格式不正确，请核实')</script>");
                    return;
                }
                finally
                {
                    sqloutID.Clear();
                    sqlspin.Clear();
                    sqlsql.Clear();

                }
            }
        }
    }
    /// <summary>
    /// 出库信息
    /// </summary>
    public class products
    {
        /// <summary>
        /// 仓库名字
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 睿配编码
        /// </summary>
        public string RpCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; set; }
    }
}
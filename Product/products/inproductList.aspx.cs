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
    public partial class inproductList : Common.BasePage
    {
        protected int curpage = 1, pagesize = 20, allCount = 0;
        protected string rpcode = "", Edate, Bdate;
        protected SJYEntity.Common.Search sinfo = new SJYEntity.Common.Search();
        protected ProductBLL.Basebll.BaseList hb = new ProductBLL.Basebll.BaseList();
        StringBuilder sqlpin = new StringBuilder();
        StringBuilder sqlinID = new StringBuilder();
        protected StringBuilder result = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            Bdate = GetQueryString("bdate");
            Edate = GetQueryString("edate");
            rpcode = GetQueryString("rpcode");
            if (!IsPostBack)
            {
                //当前人员区域权限

                // hiddtype.Value =isDistrictRights();
                hiddpart.Value = isPartcode();//角色权限编码


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


            allCount = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            lblCount.Text = allCount.ToString();

            DataTable dt = ds.Tables[1];
            dt.Columns.Add("markName");//特殊标识
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
            if (!ds.Equals(null))
            {




                dislist.DataSource = dt;
                dislist.DataBind();
                if (allCount <= pagesize)
                {
                    literalPagination.Visible = false;
                }
                else
                {
                    literalPagination.Visible = true;
                    literalPagination.Text = GenPaginationBar("inproductList.aspx?page=[page]&bdate=" + Bdate + "&edate=" + Edate + "&rpcode=" + rpcode + "", pagesize, curpage, allCount);
                }
            }
        }

        /// <summary>
        /// 如果是点击查询功能首次要把PageIndex 重置为1
        /// </summary>
        private void InitParam(string Index)
        {

            sinfo.PageSize = 20;


            // sinfo.Orderby = "id";

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


            #region
            sinfo.Sqlstr = "(SELECT dbo.inproduct.usercode, dbo.products.ShGoogcode,dbo.products.CAD, dbo.inproduct.INbatch, dbo.inproduct.id,OD ,FHDate ,SHDate , dbo.inproduct.rpcode,THData,QTY ,WHCode ,CustomerCode ,Note ,specialnote,intype,inprice,basename AS  'WHName',spmark FROM  dbo.inproduct inner  JOIN dbo.SYS_RightStore  ON WHCode=dbo.SYS_RightStore.SR_storecode  left join BaseStore on WHCode= Basecode LEFT JOIN dbo.products ON dbo.inproduct.rpcode =dbo.products.rpcode  where sp_code='" + hiddpart.Value + "' )hh";
            if (!string.IsNullOrEmpty(Bdate) && !string.IsNullOrEmpty(Edate))
            {
                sinfo.Sqlstr += " where  CONVERT(VARCHAR(20), SHDate, 23) BETWEEN '" + Bdate + "' AND  '" + Edate + "' ";
            }
            else
            {
                sinfo.Sqlstr += " where 1=1 ";
            }

            if (rpcode != "")
            {
                sinfo.Sqlstr += " and  rpcode LIKE'%" + rpcode + "%' ";
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            #region exec格式处理

            bool fileIsValid = false;
            //如果确认了文件上传，则判断文件类型是否符合要求
            if (this.FileUpload1.HasFile)
            {
                //获取上传文件的后缀名
                String fileExtension = System.IO.Path.GetExtension(this.FileUpload1.FileName).ToLower();//ToLower是将Unicode字符的值转换成它的小写等效项

                //判断文件类型是否符合要求
                //val.Value = fileExtension;

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
                    string name = Server.MapPath("~/uploadxls/") + "IN" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "") + ".csv";
                    this.FileUpload1.SaveAs(name);
                    if (File.Exists(name))
                    {
                        string tsbs = "";


                        DataTable dt = ProductBLL.Search.Searcher.OpenCSV(name);
                        if (dt.Rows.Count > 0)
                        {
                            dt.Columns.Add("smark");

                            string usercode = userCode();
                            string storecode = "";

                            int s = 0;//根据不同数字解析对应的结果
                            #region 判断
                            DataTable isrpcode = hb.getdate("rpcode", "dbo.products");  //可操作的仓库编码
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                storecode = dt.Rows[i]["仓库编码"].ToString();

                                #region 睿配编码是否存在


                                DataRow[] drr = isrpcode.Select("rpcode='" + dt.Rows[i]["睿配编码"].ToString() + "'");
                                if (drr.Length <= 0)
                                {

                                    s = 7;
                                    sqlinID.Append(dt.Rows[i]["睿配编码"].ToString() + ",");
                                    break;
                                }

                                #endregion


                                #region 根据角色判断是否有该仓库权限 4
                                DataTable dtstore = hb.getdate("SR_storecode", "dbo.SYS_RightStore WHERE  SP_code='" + hiddpart.Value + "'");  //可操作的仓库编码

                                DataRow[] dr = dtstore.Select("SR_storecode='" + storecode + "'");
                                if (dr.Length <= 0)
                                {

                                    s = 4;
                                    sqlinID.Append(storecode + ",");
                                    break;
                                }

                                #endregion
                                #region 标识c处理
                                tsbs = dt.Rows[i]["特殊标识"].ToString().Trim();

                                if (tsbs != "")
                                {
                                    switch (tsbs)
                                    {
                                        case "正常入库":
                                            break;

                                        case "退货入库":
                                            tsbs = "3";

                                            break;

                                        case "特殊入库":
                                            tsbs = "8";
                                            break;
                                        case "仓库调拨":
                                            tsbs = "4";
                                            break;
                                        default:
                                            tsbs = "未知";
                                            break;
                                    }
                                    if (tsbs == "未知")
                                    {

                                        s = 5;//标识不存在需要在webconfig配置
                                        break;

                                    }
                                    if (tsbs == "8")
                                    {
                                        if (dt.Rows[i]["特殊描述"].ToString() == "")
                                        {

                                            s = 6;
                                            break;
                                        }
                                    }
                                    if (tsbs == "4")
                                    {

                                        s = 4;//取消调拨入库
                                        break;

                                    }
                                }
                                #endregion
                            }

                            #endregion

                            #region 根据返回数字来解析相应的结果


                            if (s == 5)
                            {
                                //上传失败的话 要把已经上传成功的数据删除掉
                                Response.Write("<script type='text/javascript'>window.parent.alert('上传失败,系统无此特殊标识:" + tsbs + "');window.location.href='../products/inproductList.aspx';</script>");
                                return;
                            }

                            else if (s == 6)
                            {
                                //上传失败的话 要把已经上传成功的数据删除掉
                                Response.Write("<script type='text/javascript'>window.parent.alert('上传失败,特殊标识是特殊入库,退货入库时特殊说明不能为空!');window.location.href='../products/inproductList.aspx';</script>");
                                return;
                            }
                            else if (s == 7)
                            {
                                Response.Write("<script type='text/javascript'>window.parent.alert('睿配编码不存在:" + sqlinID.ToString() + "');</script>");
                                return;
                            }
                            else if (s == 4)
                            {
                                Response.Write("<script type='text/javascript'>window.parent.alert('已取消调拨入库，请执行调拨');</script>");
                                return;
                            }
                            #endregion


                            #region 批次号
                            //LT+R/C日期 +1
                            //当天导入 数字累加
                            //LTR20151120-1


                            int numbatch = 1;
                            string day = DateTime.Today.ToString("yyyy-MM-dd");
                            string Scalar = hb.GetScalarstring("  ISNULL(MAX(batchCount),'') ", " dbo.inproduct   where CONVERT(VARCHAR(20),insettime,23)='" + day + "'");

                            if (Scalar != "")
                            {
                                numbatch = Convert.ToInt32(Scalar) + 1;
                            }

                            #endregion


                            #region  for循环处理数据


                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                storecode = dt.Rows[i]["仓库编码"].ToString();

                                string batch = dt.Rows[i]["睿配编码"].ToString().Substring(0, 2);
                                string batchnum = batch + "R" + day.Replace("-", "") + "-" + numbatch;


                                #region  版本 15.06.15 数据操作


                                #region 标识c处理
                                tsbs = dt.Rows[i]["特殊标识"].ToString().Trim();
                                string time = "";
                                if (tsbs != "")
                                {
                                    switch (tsbs)
                                    {
                                        case "正常入库":
                                            tsbs = "1";
                                            // time = dt.Rows[i]["发货日期"].ToString();

                                            break;

                                        case "退货入库":
                                            tsbs = "3";
                                            // time = dt.Rows[i]["退回日期"].ToString();
                                            break;

                                        case "特殊入库":
                                            tsbs = "8";
                                            //time = dt.Rows[i]["到库日期"].ToString();
                                            break;

                                    }


                                }
                                #endregion




                                int num = Convert.ToInt32(dt.Rows[i]["数量"].ToString());
                                //note =产品编码+发货日期+仓库编码
                                //string note = dt.Rows[i]["睿配编码"].ToString() + Convert.ToDateTime(time).ToString("yyyy/MM/dd").Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "") + storecode;
                                //变更20151215 产品编码+仓库编码
                                string note = dt.Rows[i]["睿配编码"].ToString() + storecode;

                                string sqlstr = "exec pro_instore  '" + dt.Rows[i]["睿配编码"].ToString() + "','" + dt.Rows[i]["下单日期"].ToString() + "','" + dt.Rows[i]["发货日期"].ToString() + "','" + dt.Rows[i]["到库日期"].ToString() + "','" + dt.Rows[i]["退回日期"].ToString() + "','" + num + "','" + storecode + "','" + Convert.ToInt32(tsbs) + "','" + note + "','" + dt.Rows[i]["价格"].ToString() + "' ,'" + usercode + "','','','" + dt.Rows[i]["特殊描述"].ToString() + "','" + batchnum + "','" + numbatch + "' ";



                                #endregion




                                #region 执行插入 根据返回数字解析结果



                                if (!hb.ProExecinset(sqlstr))
                                {
                                    s = 1;
                                    break;
                                }
                                #endregion



                            }
                            #endregion


                            #region 根据返回数字来解析相应的结果

                            if (s == 0)
                            {
                                Response.Write("<script type='text/javascript'>window.parent.alert('上传成功');window.location.href='../products/inproductList.aspx';</script>");
                            }
                            else if (s == 4)
                            {

                                Response.Write("<script type='text/javascript'>window.parent.alert('当前人员没有权限操作该仓库:" + sqlinID.ToString() + "');</script>");
                            }

                            else if (s == 1)
                            {

                                //上传失败的话 要把已经上传成功的数据删除掉
                                Response.Write("<script type='text/javascript'>window.parent.alert('上传失败,请删除已经上传的数据');window.location.href='../products/inproductList.aspx';</script>");


                            }
                        }



                            #endregion

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
                    sqlinID.Clear();
                }
            }
        }
        protected void btnQuerey_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.Form["rpcode"]))
            {
                rpcode = Request.Form["rpcode"].Trim();
            }
            else rpcode = "";
            if (!string.IsNullOrEmpty(Request["bdata"]))
            {
                Bdate = Request.Form["bdata"].Trim();
            }
            else Bdate = "";
            if (!string.IsNullOrEmpty(Request["edata"]))
            {
                Edate = Request.Form["edata"].Trim();
            }
            else Edate = "";
            try
            {
                InitParam("query");
                bindData();
            }
            catch { }
        }
    }
}
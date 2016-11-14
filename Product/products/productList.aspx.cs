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
        
    public partial class productList :Common.BasePage
    {

        protected string rpcode = "", code = "", model = "", styledisplay = "style=\"display: none;\"", iscity = "style=\"display: none\"";
        protected int curpage = 1, pagesize = 20, allCount = 0;
        protected SJYEntity.Common.Search sinfo = new SJYEntity.Common.Search();
        protected ProductBLL.Basebll.BaseList hb = new ProductBLL.Basebll.BaseList();
        StringBuilder sqlpin = new StringBuilder();
        StringBuilder sqlCad = new StringBuilder();//查看编码是否重复
        protected void Page_Load(object sender, EventArgs e)
        {

			if (userCode() == "admin") { styledisplay = ""; }
          DataTable dbcity=     hb.getdate("Srcode,spcode", "dbo.SYS_RightNew WHERE  spcode ='" + isPartcode() + "'");

          if (dbcity.Rows.Count > 0)//有城市权限的时候才显示
          {
              iscity = "";
           }
        
            if (!IsPostBack)
            {
                if (GetQueryString("model") != "")
                {
                    model = GetQueryString("model");
                }
                if (GetQueryString("CAD") != "")
                {
                    code = GetQueryString("CAD");
                }
                if (GetQueryString("rpcode") != "")
                {
                    rpcode = GetQueryString("rpcode");
                }
                if (GetQueryString("ischeck") != "")
                {
                    chck.Checked=true;
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
                    if (chck.Checked)
                    {
                        literalPagination.Text = GenPaginationBar("productList.aspx?page=[page]&rpcode=" + rpcode + "&CAD=" + code + "&model=" + model + "&ischeck=1", pagesize, curpage, allCount);
                    }
                    else
                    {
                        literalPagination.Text = GenPaginationBar("productList.aspx?page=[page]&rpcode=" + rpcode + "&CAD=" + code + "&model=" + model + "&ischeck=", pagesize, curpage, allCount);
                    }
                } 
            }
        }


        private void InitParam(string Index)
        {
            #region 
            sinfo.PageSize = 20;

               sinfo.Orderby = "id";
               if (chck.Checked)
               {
                  
                   sinfo.Sqlstr = "( SELECT   products.id, rpcode , ShGoogcode ,products.CAD , pinglei ,  pingpai ,   Dimension , ";
                   sinfo.Sqlstr += " AcrossWidth ,   GKB ,   R ,  Rimdia , PATTERN , Mark , ";
                   sinfo.Sqlstr += "  LOADINGs ,  SPEEDJb ,   EXTRALOAD ,Segment ,  BRAND , OE , ";
                   sinfo.Sqlstr += " des ,  products.inserttime , CAI ,  model ,   P ,  SEASON , productionplace ";
                   sinfo.Sqlstr += " FROM  dbo.products inner    JOIN dbo.PriceMaintain ON rpcode=PriceMaintain.CAD  AND isshangxian=1 ";
                 


               }
               else
               {
                   sinfo.Sqlstr = "( SELECT * FROM  dbo.products  ";  
               }
            //sinfo.Tablename = "products"; 
         
         
            if (code != "")
            {
                sinfo.Sqlstr += " where  CAD  LIKE'%" + code + "%' ";
            }
            else
            {
                sinfo.Sqlstr += " where  1=1 ";
            }
            if (rpcode != "")
            {
                sinfo.Sqlstr += "  and rpcode  LIKE'%" + rpcode + "%' ";
            }
            if (model != "")
            {
                sinfo.Sqlstr += "  and Dimension  LIKE'%" + model + "%')hh ";
            }
            else
            {
                sinfo.Sqlstr += ")hh";
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
            Querey();
            try
            {
                InitParam("query");
                bindData();
            }
            catch { }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
			//DataTable dbproduct = DataMgr.GetTableSchema();
			//DataRow drproduct;
            bool fileIsValid = false;
            //如果确认了文件上传，则判断文件类型是否符合要求
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
                /*****************************修订***************************************************
                ///功能：新插入一个字段rpcode，更具字段Dimension 才用正则来生产相应的CAD、车毂直径、花纹、型号等
                ///目的：是为了可以实时保存运行任务的执行状态
                ///完成时间：2015-04-13
                ///作者：cx
                ///遗留问题：无
                ///说明：无 
                ///版本：00.15.01
                ///修订：修订方式
                ************************************************** */
                try
                {
                    string name = Server.MapPath("~/uploadxls/") + "IN" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "") + ".csv";
                    this.FileUpload1.SaveAs(name);
                    if (File.Exists(name))
                    {


                        //   DataTable dtcsv = ProductBLL.download.ExcelHelper.ExcelToDataTable(name,true);
                       DataTable dtcsv = ProductBLL.Search.Searcher.OpenCSV(name);
                        if (dtcsv.Rows.Count > 0)
                        {   //修订版本号：00.15.04
                            //功能：采用正则来生成相应的字段
                            //完成时间：2015-04-22
                            //作者：cx
                            //说明：无 
                            #region   修订版本号：00.15.04版本循环插入表products

							// 检测睿配编码列是否有重复 如果有重复就终止操作并提示重复编码

							var ageGroups3 = from row in dtcsv.Rows.Cast<DataRow>() group row by Convert.ToString(row["睿配编码"]) into resultCollection select resultCollection;
                           
                            foreach (var group in ageGroups3)
                            {
                                if (Convert.ToInt32(group.Count().ToString()) > 1)
                                {
                                   sqlCad.Append( group.Key.ToString()+",");
                                }
                            }


                            #region 编码无重复项
                            
                            
                            if (sqlCad.Length <= 0)//没有重复项
                            {
                                //165/70R13 ENERGY XM2
                                //cad530375_106
								//string   R = "[Z|R](.*?)(?=\\d)"; //匹配车毂直径 R13 保留字母R
								//string strR = "(?<=R)\\w+(?=\\s)"; //匹配车毂直径R13 保留数字 13
								//string model = "(.*?)(?=\\s)";//匹配型号 165/70R13
								//string strCAI = "(\\d{6})(?=[_])";//根据CAD530375_106匹配CAi 截取—前数字
								//string strPattern = "(?=[ ]).*";//匹配花纹PATTERN
                                for (int i = 0; i < dtcsv.Rows.Count; i++)
                                {
                                    //165/70R13 ENERGY XM2
                                   if(i==400){
									   string ss = "";
								   }
                                    #region 正则匹配
                                    
                                    /*
									//if (dtcsv.Rows[i]["rpcode"].ToString() == "")//匹配CAi
									//{
									//     Regex ree = new Regex(strCAI, RegexOptions.IgnoreCase | RegexOptions.Multiline);
									//     MatchCollection rpcode = ree.Matches(dtcsv.Rows[i]["CAD"].ToString());

									//     dtcsv.Rows[i]["rpcode"] = rpcode[0].ToString();
									//}

                                    if (dtcsv.Rows[i]["R"].ToString() == "")//匹配R
                                    {
                                        Regex ree = new Regex(R, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        MatchCollection regR = ree.Matches(dtcsv.Rows[i]["Dimension"].ToString());

                                        dtcsv.Rows[i]["R"] = regR[0].ToString();
                                    }

                                    if (dtcsv.Rows[i]["轮辋直径"].ToString() == "")//匹配轮辋直径
                                    {
                                        Regex ree = new Regex(strR, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        MatchCollection regR = ree.Matches(dtcsv.Rows[i]["Dimension"].ToString());

                                        dtcsv.Rows[i]["轮辋直径"] = regR[0].ToString();
                                    }


                                    if (dtcsv.Rows[i]["型号"].ToString() == "")//匹配型号165/70R13
                                    {
                                        Regex ree = new Regex(model, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        MatchCollection regDES = ree.Matches(dtcsv.Rows[i]["Dimension"].ToString());
                                        dtcsv.Rows[i]["型号"] = regDES[0].ToString().TrimEnd();
                                    }
                                    if (dtcsv.Rows[i]["花纹"].ToString() == "")//匹配花纹PATTERN
                                    {
                                        Regex ree = new Regex(strPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        MatchCollection regPattern = ree.Matches(dtcsv.Rows[i]["Dimension"].ToString());
                                        dtcsv.Rows[i]["花纹"] = regPattern[0].ToString().TrimStart();
                                    }

									*/
                                    #endregion

									#region 不用

									/*
									drproduct = dbproduct.NewRow();
									drproduct[0] = dtcsv.Rows[i]["SAS PN"].ToString().Trim();
									drproduct[1] = dtcsv.Rows[i]["石化商品编码"].ToString().Trim();
                                    drproduct[2] = dtcsv.Rows[i]["Dimension"].ToString().Trim();
                                    drproduct[3] = dtcsv.Rows[i]["型号"].ToString().Trim();  
                                    drproduct[4] = dtcsv.Rows[i]["P"].ToString().Trim();
                                    drproduct[5] = dtcsv.Rows[i]["横截面宽"].ToString().Trim();
                                    drproduct[6] = dtcsv.Rows[i]["高宽比"].ToString().Trim();
                                    drproduct[7] = dtcsv.Rows[i]["R"].ToString().Trim();
                                    drproduct[8] = dtcsv.Rows[i]["轮辋直径"].ToString().Trim();
                                    drproduct[9] = dtcsv.Rows[i]["花纹"].ToString().Trim();
                                    drproduct[10] = dtcsv.Rows[i]["特别标示"].ToString().Trim();
                                    drproduct[11] = dtcsv.Rows[i]["载重指数"].ToString().Trim();
                                    drproduct[12] = dtcsv.Rows[i]["速度级别"].ToString().Trim();
                                    drproduct[13] = dtcsv.Rows[i]["EXTRA LOAD"].ToString().Trim();
                                    drproduct[14] = dtcsv.Rows[i]["OE"].ToString().Trim();
                                    drproduct[15] = dtcsv.Rows[i]["des"].ToString().Trim();
                                    dbproduct.Rows.Add(drproduct);
								 * */
									#endregion
									if (dtcsv.Rows[i]["石化商品编码"].ToString() == "0" || string.IsNullOrEmpty(dtcsv.Rows[i]["石化商品编码"].ToString())) {
										dtcsv.Rows[i]["石化商品编码"] = "";

									}
									sqlpin.Append("INSERT  INTO dbo.products ( rpcode ,ShGoogcode ,CAD ,pinglei , pingpai , Dimension ,   AcrossWidth , GKB ,   R ,   Rimdia ,  PATTERN ,  Mark ,  LOADINGs ,   SPEEDJb ,  EXTRALOAD ,Segment ,   OE , [des] ) values ");

//                                    睿配编码 石化商品编码
//供应商编码 品类	
//品牌	
//Dimension
//横截面宽 高宽比	R	轮辋直径
//花纹		特别标示	载重指数	速度级别	 
//EXTRA LOAD	Segment	OE	des
									sqlpin.Append("('" + dtcsv.Rows[i]["睿配编码"].ToString().Trim() + "','" + dtcsv.Rows[i]["石化商品编码"].ToString().Trim() + "','" + dtcsv.Rows[i]["供应商编码"].ToString().Trim() + "','" + dtcsv.Rows[i]["品类"].ToString().Trim() + "','" + dtcsv.Rows[i]["品牌"].ToString().Trim() + "','" + dtcsv.Rows[i]["Dimension"].ToString().Trim() + "','" + dtcsv.Rows[i]["横截面宽"].ToString().Trim() + "','" + dtcsv.Rows[i]["高宽比"].ToString().Trim() + "','" + dtcsv.Rows[i]["R"].ToString().Trim() + "','" + dtcsv.Rows[i]["轮辋直径"].ToString().Trim() + "','" + dtcsv.Rows[i]["花纹"].ToString().Trim() + "','" + dtcsv.Rows[i]["特别标示"].ToString().Trim() + "','" + dtcsv.Rows[i]["载重指数"].ToString() + "','" + dtcsv.Rows[i]["速度级别"].ToString() + "','" + dtcsv.Rows[i]["EXTRALOAD"].ToString().Trim() + "','" + dtcsv.Rows[i]["Segment"].ToString().Trim() + "','" + dtcsv.Rows[i]["OE"].ToString().Trim() + "','" + dtcsv.Rows[i]["des"].ToString().Trim() + "')");

                                }
                                #region 有数据执行插入


								if (sqlpin.Length > 0)
                                {

                                    if (hb.insetpro(sqlpin.ToString()))
                                   // if (DataMgr.BulkToDBProducts(dbproduct))
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传成功');window.location.href='../products/productList.aspx';</script>");
                                       // dbproduct.Clear();
                                        sqlpin.Clear();
                                        return;
                                    }
                                    else
                                    {
                                        Response.Write("<script type='text/javascript'>window.parent.alert('上传失败');</script>");
                                        sqlpin.Clear();
                                       
                                        return;
                                    }

                                }

                                #endregion
                            }
                            #endregion

                            else
                            {
								Response.Write("<script type='text/javascript'>window.parent.alert('上传失败！睿配编码重复：" + sqlCad.ToString() + "');</script>");
                                
                                sqlCad.Clear();
                                return;
                            }


                          
                            #endregion


                            
                        }
                        else
                        {
                            Response.Write("<script type='text/javascript'>window.parent.alert('文件无内容');</script>");
                          
                            //sqlpin.Clear();
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
                    //dbproduct.Dispose();
                }
            }
        }
        public void Querey()
        {
            if (!string.IsNullOrEmpty(Request.Form["model"]))
            {
                model = Request.Form["model"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Form["CAD"]))
            {
                code = Request.Form["CAD"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Form["rpcode"]))
            {
                rpcode = Request.Form["rpcode"].Trim();
            }
          
        }
    
        protected void Buttondow_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("~/downloadxlsx/Order/");
            string datetime = "产品列表" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "").ToString().Replace("-", "");

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
            string issx = "0";
            if (chck.Checked)

            {
                issx = "1";
            }

      
            //StringBuilder str = new StringBuilder();
            //str.Append("declare @sql varchar(8000)");

            //str.Append("set @sql ='SELECT *   FROM (select rpcode AS 睿配编码, ''''+ CONVERT(VARCHAR(30),ShGoogcode)  '");
            //str.Append("set @sql =@sql+'AS 石化商品编码,CAD AS 供应商编码 ,pingpai AS 品类 ,Dimension AS 规格, '");
            //str.Append("set @sql =@sql+'PATTERN AS 花纹,LOADINGs AS 载重指数,SPEEDJb AS 速度级别 '");
            //str.Append("set @sql =@sql+' FROM  dbo.products) mm INNER JOIN  '");

            ////str.Append("set @sql =@sql+ '  (select rpcode as rpcodeid '");
            ////str.Append("select @sql = @sql + ' , max(case cityname when ''' + cityname + ''' then PriceJinhuo else 0 end) [' + cityname + ']'");
            ////str.Append("from (SELECT cityname FROM  dbo.SYS_RightNew LEFT JOIN  dbo.City ON  AreaCode=Srcode WHERE spcode='" + ispartRights() + "') as a");
            ////str.Append("set @sql = @sql + ' from #b group by rpcode) kk ON mm.睿配编码 = kk.rpcodeid'");


            //str.Append("exec(@sql)");
            string strsql = "exec dowproducts '" + model + "','" + code + "','" + rpcode + "','" + issx + "','" + isPartcode() + "'";
           // strsql += "	, 	pingpai AS '品类' ,Dimension AS '规格' ,  ";
           //strsql += "	PATTERN AS '花纹',LOADINGs AS '载重指数',SPEEDJb AS '速度级别'  ";

            DataTable dtdb = hb.getProdatable(strsql);
            dtdb.Columns.Remove("rpcodeid");
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
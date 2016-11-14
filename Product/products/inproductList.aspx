<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inproductList.aspx.cs"
    Inherits="product.products.inproductList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../Styles/dict.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/time/Calendar/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .ui-autocomplete
        {
            max-height: 200px;
            overflow-y: auto; /* 防止水平滚动条 */
            overflow-x: hidden;
            background-color: #CCCCCC;
            width: 200px;
        }
    </style>
    <script>

        $(function () {
           var availableTags =<%=result%>;
            $("#rpcode").autocomplete({
                source: availableTags
            });

            $("#rpcode").data("autocomplete")._renderItem = function(ul, item) {
					return $("<li></li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
				};
				

        });
    </script>
    <script type="text/javascript">

        $(function () {
            $.each($("input[name=bdata]"), function () {
                $(this).focus(function () { WdatePicker({ maxDate: '#F{$dp.$D(\'edata\')||\'2020-10-01\'}', readOnly: true }); });
                $(this).click(function () { WdatePicker({ maxDate: '#F{$dp.$D(\'edata\')||\'2020-10-01\'}', readOnly: true }); });

            });
            $.each($("input[name=edata]"), function () {

                $(this).focus(function () { WdatePicker({ minDate: '#F{$dp.$D(\'bdata\')}', maxDate: '2020-10-01', readOnly: true }); });
                $(this).click(function () { WdatePicker({ minDate: '#F{$dp.$D(\'bdata\')}', maxDate: '2020-10-01', readOnly: true }); });
            });

            if ($("#hiddpart").val() == "信息查看员") {
                document.getElementById("addp").style.display = "none";

            }
        });
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="subnav">
        进库列表
    </div>
    <div id="right">
        <div class="page ">
            <p class="left" id="addp">
                <a href="../products/addin.aspx"><strong>进库新增</strong></a> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <%--<input runat="server" id="val"/>--%>
                <asp:Button ID="shangchuan" runat="server" OnClick="Button1_Click" Text="上　传" Width="54px" />
            </p>
            <p class="right">
                每页<%=pagesize%>条 &nbsp; 符合条件的记录有<strong style="color: Red"><asp:Label ID="lblCount"
                    runat="server"></asp:Label></strong>条</p>
            <div class="clear">
            </div>
            <div class="page ">
                <strong>开始日期</strong>
                <input id="bdata" name="bdata" type="text" value="<%=Bdate %>" />
                <strong>结束日期</strong>
                <input id="edata" name="edata" type="text" value="<%=Edate %>" />
                <strong>睿配编码</strong>
                <input id="Rpcode" name="rpcode" type="text" value="<%=rpcode %>" />
                <asp:Button runat="server" ID="btnQuerey" Text="查询" OnClick="btnQuerey_Click" />
            </div>
        </div>
        <asp:Repeater runat="server" ID="dislist">
            <HeaderTemplate>
                <table id="list">
                    <tr>
                        <th width="3%">
                            ID
                        </th>
                        <th width="8%">
                            睿配编码
                        </th>
                        <th width="8%">
                            石化商品编码
                        </th>
                        <th width="10%">
                            供应商编码
                        </th>
                        <th width="5%">
                            下单日期
                        </th>
                        <th width="5%">
                            发货日期
                        </th>
                        <th width="5%">
                            到货日期
                        </th>
                        <th width="5%">
                            退货日期
                        </th>
                        <th width="3%">
                            数量
                        </th>
                        <th width="10%">
                            仓库名
                        </th>
                        <th width="5%">
                            特殊标示
                        </th>
                        <th width="5%">
                            批次号
                        </th>
                        <th width="5%">
                            操作人
                        </th>
                        <%--  <th width="8%">
                                类型
                            </th>--%>
                        <%--  <th width="25%">
                                其他
                            </th>--%>
                        <%-- <th width="20%">
                                特殊描述
                            </th>--%>
                        <th width="8%">
                            操作
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr id="base<%#Eval("id")%>">
                    <td>
                        <%#Eval("id")%>
                    </td>
                    <td>
                        <%#Eval("rpcode")%>
                    </td>
                    <td>
                        <%#Eval("ShGoogcode")%>
                    </td>
                    <td>
                        <%#Eval("CAD")%>
                    </td>
                    <td>
                        <%#Eval("OD", "{0:yyyy-MM-dd}")%>
                    </td>
                    <td>
                        <%#Eval("FHDate", "{0:yyyy-MM-dd}")%>
                    </td>
                    <td>
                        <%#Eval("SHDate", "{0:yyyy-MM-dd}")%>
                    </td>
                    <td>
                        <%#Eval("THData", "{0:yyyy-MM-dd}")%>
                    </td>
                    <td>
                        <%#Eval("QTY")%>
                    </td>
                    <td>
                        <%#Eval("WHName")%>
                    </td>
                    <td>
                        <%#Eval("markName")%>
                    </td>
                    <td>
                        <%#Eval("INbatch")%>
                    </td>
                    <td>
                        <%#Eval("usercode")%>
                    </td>
                    <%--   <td>
                            <%#Eval("intype")%>
                        </td>--%>
                    <%--   <td>
                            <%#Eval("Note")%>
                            <br />
                             <%#Eval("specialnote")%>
                        </td>--%>
                    <td>
                        <a href="../products/updateinpro.aspx?id=<%#Eval("id")%>">编辑</a>| <a href="javascript:del('<%#Eval("id")%>')">
                            删除</a>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table></FooterTemplate>
        </asp:Repeater>
        <div class="Pages">
            <div class="Paginator">
                <asp:Literal ID="literalPagination" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hiddtype" runat="server" />
    <asp:HiddenField ID="hiddpart" runat="server" />
    <script type="text/javascript">
        function del(id) {
            if (confirm("是否删除！")) {
                $.post("../Handler/delstoreck.ashx?table=BaseStore&type=0&id=" + id, {}, function (data) {
                    if (data.result == "100") {
                        alert("删除成功！");
                        $("#base" + id)[0].style.display = "none";
                    }
                    else
                    { alert("删除失败！"); }
                }, "json");
            }
        }
    </script>
    </form>
</body>
</html>

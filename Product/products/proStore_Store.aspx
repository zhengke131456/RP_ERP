<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="proStore_Store.aspx.cs"
    Inherits="product.products.proStore_Store" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../Styles/dict.css" rel="stylesheet" type="text/css" />
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
            $("#QCAI").autocomplete({
                source: availableTags
            });

            $("#QCAI").data("autocomplete")._renderItem = function(ul, item) {
					return $("<li></li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
				};
				

        });
    </script>
</head>
<body>
   
    <form id="form1" runat="server">
     <asp:HiddenField ID="Hidpart" runat="server" />
    <div class="subnav">
        库存信息
    </div>
    <div id="right">
        <div class="page ">
            <p class="right">
                每页<%=pagesize%>条 &nbsp; 符合条件的记录有<strong style="color: Red"><asp:Label ID="lblCount"
                    runat="server"></asp:Label></strong>条</p>
            <div class="clear">
            </div>
            <div class="page ">
                <strong>睿配编码</strong>
                <input id="QCAI" name="QCAI" type="text" value="<%=cai %>" />
                <strong>仓库名</strong>
                <%--<input id="strockName" name="strockName" type="text" value="<%=strockName %>"   />--%>
                <asp:DropDownList Width="200" ID="dpstoreNew" runat="server">
                </asp:DropDownList>
                <strong>型号</strong>
                <asp:DropDownList Width="200" ID="dimension" runat="server">
                </asp:DropDownList>
                <asp:Button runat="server" ID="btnQuerey" Text="查询" OnClick="btnQuerey_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button1" runat="server" OnClick="Buttondow_Click" Text="下载" Width="54px" />
            </div>
        </div>
        <div style="overflow: auto; width: 100%; height: 550px;">
            <asp:Repeater runat="server" ID="dislist">
                <HeaderTemplate>
                    <table width="100%" id="list">
                        <tr>
                            <th width="5%">
                                ID
                            </th>
                            <th width="10%">
                                睿配编码
                            </th>
                            <th width="10%">
                                石化商品编码
                            </th>
                            <th width="10%">
                                供应商编码
                            </th>
                            <th width="10%">
                                型号
                            </th>
                            <th width="10%">
                                花纹
                            </th>
                            <th width="10%">
                                库存量
                            </th>
                            <th width="10%">
                                仓库编码
                            </th>
                            <th width="10%">
                                仓库
                            </th>
                            <th width="50%">
                                操作
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="base<%#Eval("ID")%>">
                        <td>
                            <%#Eval("ID")%>
                        </td>
                        <td>
                            <%#Eval("rpcode")%>
                        </td>
                        <td>
                            <%#Eval("ShGoogcode")%>
                        </td>
                        <td>
                            <%#Eval("cad")%>
                        </td>
                        <td>
                            <%#Eval("Dimension")%>
                        </td>
                        <td>
                            <%#Eval("PATTERN")%>
                        </td>
                        <td>
                            <%#Eval("stockNum")%>
                        </td>
                        <td>
                            <%#Eval("Basecode")%>
                        </td>
                        <td>
                            <%#Eval("stockName")%>
                        </td>
                        <td>
                            <a href="../products/ProductInStore.aspx?storeID=<%#Eval("stockID")%>&CAI=<%#Eval("rpcode")%>">
                                入库明细</a>| <a href="../products/ProductOutStore.aspx?storeID=<%#Eval("stockID")%>&CAI=<%#Eval("rpcode")%>">
                                    出库明细</a>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table></FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="Pages">
            <div class="Paginator">
                <asp:Literal ID="literalPagination" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
    </form>
</body>
</html>

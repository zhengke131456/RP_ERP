<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Transferslip.aspx.cs" Inherits="product.products.Transferslip" %>

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
    <script type="text/javascript">

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
</head>
<body>
    <form id="form1" runat="server">
    <div class="subnav">
        调拨单
    </div>
    <div id="right">
        <div class="page ">
            <p class="left">
                <a href="../products/proStore_Transfer.aspx"><strong>新建调拨单</strong></a> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <%--            <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="shangchuan" runat="server"  OnClick="Button1_Click" Text="上　传" Width="54px" />--%>
            </p>
            <p class="right">
                每页<%=pagesize%>条 &nbsp; 符合条件的记录有<strong style="color: Red"><asp:Label ID="lblCount"
                    runat="server"></asp:Label></strong>条</p>
            <div class="clear">
            </div>
            <div class="page ">
                <strong>睿配编码</strong>
                <input id="rpcode" name="rpcode" type="text" value="<%=rpcode %>" />
                <strong>出库仓库</strong>
                <input id="outwh" name="outwh" type="text" value="<%=outwh %>" />
                <strong>入库仓库</strong>
                <input id="inwh" name="inwh" type="text" value="<%=inwh %>" />
                <asp:Button runat="server" ID="btnQuerey" Text="查询" OnClick="btnQuerey_Click" />
            </div>
        </div>
        <div style="width: 100%; overflow: auto; height: 550px">
            <asp:Repeater runat="server" ID="dislist">
                <HeaderTemplate>
                    <table id="list" style="width: 130%">
                        <tr>
                            <th width="3%">
                                ID
                            </th>
                            <th width="5%">
                                睿配编码
                            </th>
                            <th width="8%">
                                石化商品编码
                            </th>
                            <th width="8%">
                                供应商编码
                            </th>
                            <th width="6%">
                                型号
                            </th>
                            <th width="6%">
                                花纹
                            </th>
                            <th width="5%">
                                数量
                            </th>
                            <th width="6%">
                                状态
                            </th>
                            <th width="10%">
                                出库仓库
                            </th>
                            <th width="6%">
                                出库日期
                            </th>
                            <th width="5%">
                                出库人员
                            </th>
                            <th width="10%">
                                入库仓库
                            </th>
                            <th width="8%">
                                入库日期
                            </th>
                            <th width="5%">
                                入库人员
                            </th>
                            <th width="5%">
                                创建人
                            </th>
                            <%--<th width="15%">
                            备注
                        </th>--%>
                            <th width="8%">
                                创建日期
                            </th>
                            <th width="100%">
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
                            <%#Eval("Dimension")%>
                        </td>
                        <td>
                            <%#Eval("PATTERN")%>
                        </td>
                        <td>
                            <%#Eval("QTY")%>
                        </td>
                        <td>
                            <%#Eval("states")%>
                        </td>
                        <td>
                            <%#Eval("outwhName")%>
                        </td>
                        <td>
                            <%#Eval("outwhtime", "{0:yyyy-MM-dd}")%>
                        </td>
                        <td>
                            <%#Eval("outOpcode")%>
                        </td>
                        <td>
                            <%#Eval("inWHName")%>
                        </td>
                        <td>
                            <%#Eval("inwhtime", "{0:yyyy-MM-dd}")%>
                        </td>
                        <td>
                            <%#Eval("inOpcode")%>
                        </td>
                        <td>
                            <%#Eval("opcode")%>
                        </td>
                        <%--  <td>
                        <%#Eval("note")%>
                    </td>--%>
                        <td>
                            <%#Eval("inserttime", "{0:yyyy-MM-dd}")%>
                        </td>
                        <td>
                            <div style="display: <%# Eval("states").ToString()=="待出库"?" ":"none" %>">
                                <div style="display: <%# Eval("iswh").ToString()=="0"?" ":"none" %>">
                                    <a href="javascript:outstore('<%#Eval("id")%>','<%#Eval("outWH")%>')">出库</a> | <a
                                        href="javascript:del('<%#Eval("id")%>')">删除</a>
                                </div>
                            </div>
                            <div style="display: <%# Eval("states").ToString()=="待入库"?" ":"none" %>">
                                <div style="display: <%# Eval("iswh").ToString()=="0"?" ":"none" %>">
                                    <a href="javascript:instore('<%#Eval("id")%>','<%#Eval("inWH")%>')">入库</a>
                                </div>
                            </div>
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
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function CalcShowModalDialogLocation(dialogWidth, dialogHeight) {
        var iWidth = dialogWidth;
        var iHeight = dialogHeight;
        var iTop = (window.screen.availHeight - 20 - iHeight) / 2;
        var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;
        return 'dialogWidth:300px;dialogHeight:200px;dialogTop: ' + iTop + 'px; dialogLeft: ' + iLeft + 'px;center:yes;scroll:no;status:no;resizable:0;location:no';
    }
    function del(id) {
        if (confirm("是否删除！")) {
            $.post("../delete/delete.aspx?table=transferslip&id=" + id, {}, function (data) {
                if (data.result == "100") {
                    alert("删除成功！");

                    $("#base" + id)[0].style.display = "none";
                }
                else
                { alert("删除失败！\r\n可能该调拨已出库，请刷新后重试。"); }
            }, "json");
        }
    }


    function instore(id, inwh) {
        var DialogLocation = CalcShowModalDialogLocation(500, 260);
        var Trans = new Object();
        Trans.id = id.toString();
        Trans.wh = inwh;
        Trans.type = "0";
        // var getv = window.showModalDialog("../products/popupmessage.aspx", Trans, "dialogWidth=300px;dialogHeight=200px; center:yes ;scroll: no;");
        //弹出页面获取时间
        var value = window.showModalDialog("../products/popupmessage.aspx", window, DialogLocation);
//        if (!returnValue) {
//            returnValue = window.returnValue;
//        }
        //        if (getv != "undefined") {
        //            alert(getv);
        //            window.location.reload();
        //        }
        //        else{
        //        $.post("../products/popupmessage.aspx", function (data) {
        //            if (data != null) {
        //                var str = data.msg;
        //                alert(data.msg);
        //            }
        //            else {
        //var value = prompt("请输入出库日期");

        if (value == '') {
            alert('入库日期不能为空，请重新输入！');

        } else {

            $.post("../Handler/warehouse.ashx?type=0&id=" + id, { date: value, wh: inwh }, function (data) {
                if (data.result == "0") {
                    alert("入库成功！");
                    window.location.reload();
                }
                if (data.result == "3") {
                    alert("无权限操作该仓库！");
                    window.location.reload();
                }

                if (data.result == "2") {
                    alert("入库失败！");
                    window.location.reload();
                }
                if (data.result == "4") {
                    alert("调拨单已入库或被删除\r\n请先刷新");
                    window.location.reload();
                }
            }, "json");
        }
        //        }        
    }

    function outstore(id, outwh) {
        var DialogLocation = CalcShowModalDialogLocation(500, 260);
        var Trans = new Object();
        Trans.id = id.toString();
        Trans.wh = outwh;
        Trans.type = "1";
        //弹出页面获取时间
        var value = window.showModalDialog("../products/popupmessage.aspx", window, DialogLocation);
//        if (!returnValue) {
//            returnValue = window.returnValue;
//        }
        //        if (typeof (getv) != "undefined") {
        //            alert(getv);
        //            window.location.reload();

        //        }
        //        else {

        //            var value = prompt("请输入出库日期");

        if (value == '') {
            alert('出库日期不能为空，请重新输入！');

        } else {

            $.post("../Handler/warehouse.ashx?type=1&id=" + id, { date: value, wh: outwh }, function (data) {
                if (data.result == "0") {
                    alert("出库成功！");
                    window.location.reload();
                }
                if (data.result == "3") {
                    alert("无权限操作该仓库！");
                    window.location.reload();
                }

                if (data.result == "2") {
                    alert("出库失败！");
                    window.location.reload();
                }
                if (data.result == "4") {
                    alert("调拨单已出库或被删除\r\n请先刷新");
                    window.location.reload();
                }
            }, "json");
        }
        //        }
    }
</script>

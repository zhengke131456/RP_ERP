﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="updatezshstore.aspx.cs"
    Inherits="product.baseinfo.updatezshstore" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Styles/dict.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function check() {

            if ($("#dpprovince").val() == "请选择省份") {
                alert("省份不能为空");
                return false;
            }
            if ($("#dptown").val() == "请选择城市") {
                alert("城市不能为空");
                return false;
            }
            if ($("#name").val() == "") {
                alert("仓库名不能为空");
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" name="form1" method="post" runat="server">
    <div class="subnav">
        修改
    </div>
    <div id="right">
        <table width="100%" cellpadding="2" id="shenhe">
            <tr>
                <th width="20%">
                    上级仓库编码：
                </th>
                <td>
                    <input id="storecode" type="text" name="storecode" runat="server" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    编码：
                </th>
                <td>
                    <input id="code" type="text" name="code" runat="server" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    仓库名称：
                </th>
                <td>
                    <input id="name" type="text" name="name" runat="server" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    省份：
                </th>
                <td>
                    <asp:DropDownList Width="180px" ID="dpprovince" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="dpprovince_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th width="20%">
                    城市：
                </th>
                <td>
                    <asp:DropDownList Width="180px" ID="dptown" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th width="20%">
                    销售类型：
                </th>
                <td>
                    <asp:DropDownList Width="180px" ID="dpselltype" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th width="20%">
                    状态：
                </th>
                <td>
                    <asp:DropDownList Width="180px" ID="dpstatus" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th width="20%">
                    站长：
                </th>
                <td>
                    <input id="ChinazcomName" type="text" name="ChinazcomName" runat="server" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    电话：
                </th>
                <td>
                    <input id="phone" type="text" name="phone" runat="server" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    详细地址：
                </th>
                <td>
                    <input id="address" type="text" name="address" runat="server" />
                </td>
            </tr>
            <tr id="tr1">
                <th width="20%">
                    区域：
                </th>
                <td>
                    <asp:DropDownList Width="180px" ID="dparea" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trid">
                <th width="50%">
                    备注：
                </th>
                <td>
                    <input id="note" type="text" name="note" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3" bgcolor="#F3F7F9" align="center">
                    <a style="color: Red;" href="javascript:history.go(-1)">返回</a> &nbsp;
                    <asp:Button ID="Button1" runat="server" Text="提   交" OnClick="Button1_Click" OnClientClick="return check()" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hiddID" runat="server" />
    </form>
</body>
</html>

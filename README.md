# Proxerino
Socks v5 server for WinRT 8.1+ devices - open-sourced for debugging purposes :c

Steps to run/debug this app:


1. Connect to your WLAN on your Windows Phone
2. Find out your phone internal IP address and set it in "Socks Universal/Config.cs"
![Screenshot](http://i.imgur.com/C9IAmJF.png)
3. (Optional) Force usage of cellular data by proxy server in Config.cs
4. In Firefox network setting select SOCKSv5, and enter SOCKS server address + default port of 8080
![Screenshot](http://i.imgur.com/rC9Tvgn.png)
5. Enjoy deteriorating performance :---DDDDDDD


This project uses some code licensed under LGPL3 by Benton Stark
https://github.com/bentonstark/starksoft-aspen

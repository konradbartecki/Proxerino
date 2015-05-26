# Proxerino
Currently working SOCKS v5 server for Windows and Windows Phone 8.1+ open-sourced for debugging purposes. It's not stable although it allows for basic network access, so if you are looking for some free proxy server for WP or WinRT feel free to compile it on your own.
It's made around Windows.Networking.Sockets in Portable Class Library (PCL) so it can run on all WinRT 8.1+ devices.

Steps to run/debug this app:

1. Connect to your WLAN on your Windows Phone
2. (Optional) Force usage of cellular data by proxy server in Config.cs
3. Build and run the app - server starts listening automatically. Watch VS output for some useful info
4. In Firefox network setting select SOCKSv5, and enter SOCKS server address + default port of 8080
![Screenshot](http://i.imgur.com/rC9Tvgn.png)
5. Browse web and enjoy deteriorating performance :---DDDDDDD

Usage scenario:

Let's say you have router, Windows Phone and desktop PC.
You have data plan on WP and your PC is connected to the router through ethernet cable. WAN connection to the router goes down so you don't have access to the internet on your PC anymore and you can't share your WP data plan because your PC can't access wi-fi networks. Here comes the Proxerino which allows for internet sharing and staying in your current WLAN without creating new wifi network unlike the bulit-in WP feature. Just join the WLAN network your PC is in and start the app, get the WP's IP address and configure your Firefox to connect to your WP.

This project uses some snippets licensed under LGPL3 by Benton Stark
https://github.com/bentonstark/starksoft-aspen

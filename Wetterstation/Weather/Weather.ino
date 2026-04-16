#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClientSecure.h> // Für HTTPS
//#include <ArduinoJson.h>      // Library für JSON-Verarbeitung
#include <ESP8266WebServer.h>
#include <FS.h>
#include <LittleFS.h>


const char* ssid = "Der Dreckige Dan V3";         // WLAN-Name
const char* password = "Barre#Alster"; // WLAN-Passwort;
float data, temp;
WiFiClient client;
HTTPClient http;
int SENDIT =0;
int messungen=0;
String url = "http://192.168.178.28:8080/api/data";
int luft;
ESP8266WebServer server(80);  // Webserver auf Port 80


void handlestats(){
  server.sendHeader("Access-Control-Allow-Origin", "*");
  File file = LittleFS.open("/stats.html", "r");
  if (!file) {
    server.send(500, "text/plain", "Datei nicht gefunden");
    return;
  }
  server.streamFile(file, "text/html");
  file.close();
}

void handleRoot() {
server.sendHeader("Access-Control-Allow-Origin", "*");
File file = LittleFS.open("/index.html", "r");
  if (!file) {
    server.send(500, "text/plain", "Datei nicht gefunden");
    return;
  }
  server.streamFile(file, "text/html");
  file.close();
  }

void setup() {
  Serial.begin(115200);
  WiFi.begin(ssid, password);

  // Warten auf WLAN-Verbindung
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nMit WLAN verbunden!");
  server.on("/index", handleRoot);
  server.on("/stats.html", HTTP_GET, []() {
  File file = LittleFS.open("/stats.html", "r");
  if (!file) {
    server.send(500, "text/plain", "Datei nicht gefunden");
    return;
  }
  server.sendHeader("Access-Control-Allow-Origin", "*");
  server.streamFile(file, "text/html");
  file.close();
});;
server.on("/graph", HTTP_GET, []() {
  File file = LittleFS.open("/graph.html", "r");
  if (!file) {
    server.send(500, "text/plain", "Datei nicht gefunden");
    return;
  }
  server.sendHeader("Access-Control-Allow-Origin", "*");
  server.streamFile(file, "text/html");
  file.close();
});;
  server.begin();
  Serial.println("HTTP Server gestartet");
   if (!LittleFS.begin()) {
    Serial.println("LittleFS Mount fehlgeschlagen");
    return;
  }

}
void senddata(float temp, int luft){
    http.begin(client,url);  // deine Ziel-API
    http.addHeader("Content-Type", "application/json");
    String payload = "{";
    payload += "\"temperature\":" + String(temp, 2) + ",";
    payload += "\"luftfeuchte\":" + String(luft);
    payload += "}";

    int httpResponseCode = http.POST(payload);
    Serial.print("Daten gesendet, Statuscode: ");
    Serial.println(httpResponseCode);
    http.end();
  
}


void loop() {
  if (WiFi.status() == WL_CONNECTED) {
    if (Serial.available()) {
      String input = Serial.readStringUntil('\n');
      input.trim();
      if (input.charAt(0)=='T'){
        input.remove(0,1);
        temp = input.toFloat();
        SENDIT++;
      }else if(input.charAt(0)=='H'){
        input.remove(0,1);
        luft = input.toInt();
        SENDIT++;
      }
      if(SENDIT==2){
        senddata(temp,luft);
        SENDIT=0;
        messungen++;
        if (messungen >=40){
          delay(2000);
          ESP.restart();
        }
      }
      data = input.toFloat();
      Serial.print("Empfangen: ");
      Serial.println(data);
    }
    server.handleClient(); // Webserver bedienen
  } else {
    // Optional: Reconnect versuchen
    WiFi.begin(ssid, password);
  }
}

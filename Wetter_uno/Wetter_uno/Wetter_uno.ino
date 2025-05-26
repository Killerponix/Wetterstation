#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>

DHT_Unified dht(9, DHT11);

void setup() {
  Serial.begin(115200);
  //Serial.begin(9600);
  pinMode(9, INPUT);
  pinMode(LED_BUILTIN,OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);
  dht.begin();

  sensor_t sensor;
  
  Serial.println(F("------------------------------------"));
  Serial.println(F("Temperatursensor"));
  dht.temperature().getSensor(&sensor);
  Serial.print(F("Sensor Type: ")); Serial.println(sensor.name);
  Serial.print(F("Treiber Ver: ")); Serial.println(sensor.version);
  Serial.print(F("Einzigartige ID: ")); Serial.println(sensor.sensor_id);
  Serial.print(F("Maximaler Wert: ")); Serial.print(sensor.max_value); Serial.println(F("°C"));
  Serial.print(F("Minimaler Wert: ")); Serial.print(sensor.min_value); Serial.println(F("°C"));
  Serial.print(F("Auflösung: ")); Serial.print(sensor.resolution); Serial.println(F("°C"));
  Serial.println(F("------------------------------------"));
  
  Serial.println(F("Luftfeuchtigkeitssensor"));
  dht.humidity().getSensor(&sensor);
  Serial.print(F("Sensor Type: ")); Serial.println(sensor.name);
  Serial.print(F("Treiber Ver: ")); Serial.println(sensor.version);
  Serial.print(F("Einzigartige ID: ")); Serial.println(sensor.sensor_id);
  Serial.print(F("Maximaler Wert: ")); Serial.print(sensor.max_value); Serial.println(F("%"));
  Serial.print(F("Minimaler Wert: ")); Serial.print(sensor.min_value); Serial.println(F("%"));
  Serial.print(F("Auflösung: ")); Serial.print(sensor.resolution); Serial.println(F("%"));
  Serial.println(F("------------------------------------"));
}

void loop() {
  //int TXSpannung = analogRead(A0);
  //int Spannung_3v3 = analogRead(A1);
  int Read = digitalRead(9);
  
  //float spannung = TXSpannung * (5.0 / 1023.0);
  //float spannung3v3 = Spannung_3v3 * (5.0 / 1023.0);

  sensors_event_t event;

  dht.temperature().getEvent(&event);
  if (isnan(event.temperature)) {
    //Serial.println(F("Fehler beim Ablesen der Temperatur!"));
  } else {
    //Serial.print(F("Temperatur: "));
    Serial.print('T');
    Serial.println(event.temperature);
    //Serial.println(F("°C"));
  }
  delay(100);

  dht.humidity().getEvent(&event);
  if (isnan(event.relative_humidity)) {
    //Serial.println(F("Fehler beim Ablesen der Luftfeuchtigkeit!"));
  } else {
    //Serial.print(F("Luftfeuchtigkeit: "));
    Serial.print('H');
    Serial.println(event.relative_humidity);
    //Serial.println(F("%"));
  }

  delay(60000*5); // wait 5 seconds
}

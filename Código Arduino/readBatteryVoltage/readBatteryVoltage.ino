const int pinDivisor = A0;  // Pin analógico conectado al punto medio del divisor de tensión
const float Vref = 5.0;     // Voltaje de referencia del Arduino


void setup() {
  // Habilita el puerto serie a 9600 baudios/sec
  Serial.begin(9600);
}

void loop() {
  // Habilitar el pin analógico para leer los valores de tensión en RAW
  int rawValue = analogRead(pinDivisor);
  // Transformación de los valores en digital a valores de tensión
  float voltage = rawValue * (Vref / 1023.0);

  // Escritura a traves de puerto serie los valores leidos
  Serial.print("Raw Value: ");
  Serial.print(rawValue);
  Serial.print(", Voltage: ");
  Serial.println(voltage);

  delay(1000);  // Puedes ajustar este valor según tus necesidades
}

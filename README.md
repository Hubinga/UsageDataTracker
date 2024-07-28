# Verwendete Sicherheitsprinzipien:

# 1. Eingabevalidierung: 
- Alle Benutzereingaben werden überprüft, um sicherzustellen, dass sie den erwarteten Formaten entsprechen und keine schädlichen Daten enthalten.
- Verhinderung von Injection-Angriffen: Durch die Verwendung von regulären Ausdrücken wird sichergestellt, dass nur erlaubte Zeichen akzeptiert werden.
- Sicheres Passwort: Anforderungen an die Passwortkomplexität und -länge stellen sicher, dass die Passwörter schwer zu erraten und widerstandsfähiger gegen Angriffe sind.
	 
# 2. Zugriffskontrolle (RBAC): 
- Zugriffskontrolle (RBAC): Überprüfen der Benutzerrolle und Weiterleitung basierend auf der Rolle
- Dies stellt sicher, dass nur berechtigte Benutzer Zugriff auf bestimmte Seiten und Funktionen haben
					  
# 3. Verwendung von HTTPS		  
- Verwendung von HTTPS stellt sicher, dass die Kommunikation verschlüsselt ist und vor Abhören und Manipulation geschützt wird.
- HTTPS verschlüsselt die Daten während der Übertragung, wodurch sie vor Abhören und Manipulation durch Dritte (z.B. Man-in-the-Middle-Angriffe) geschützt sind.
 
# 4. Sicheres speichern der Passwörter
- Passwörter werden nicht im Klartext gespeichert, sondern als Hash-Wert
- Der Salt ist ein zufällig generierter Wert, der dem Passwort hinzugefügt wird, bevor der Hash berechnet wird
- Selbst wenn zwei Benutzer dasselbe Passwort wählen, führen unterschiedliche Salt-Werte zu unterschiedlichen Hashes. Dies erschwert es Angreifern, Rainbow-Table-Angriffe oder andere Vorverrechnungsangriffe durchzuführen
  
# 5. 2FA
- Senden eines otp an die Email des Nutzers
- Besserer Schutz, da Angreifer eine weitere Hürde überwinden muss
- richtige Verwwendung eines one-time-password
- Löschen des Codes aus der User Tabelle, um sicherzustellen, dass der Code wirklich nur einmal verwendet werden kann.
				 		 
# 6. Sichere und effiziente Authentifizierung und Autorisierung
- sichere und effiziente Authentifizierung und Autorisierung von Benutzern innerhalb der API
- die Rolle des Benutzers im JWT festgelegt ist und zusätzliche Berechtigungen durch manuelle Zuweisung vergeben werden können
- Dadurch wird sichergestellt, dass nur gültige, nicht abgelaufene JWTs für die Authentifizierung verwendet werden
- Der Server überprüft bei jeder Anfrage den JWT, um sicherzustellen, dass es gültig und unverändert ist

# 7. Zugriffskontrolle für die API mittels CORS
- Schutz vor CSRF-Angriffen
- erlaubt nur verifizierten Anwendungen die Kommunikation

# 8. Vermeidung der festen Codierung von Schlüsseln und Anmeldedaten
- Speichern der Werte in Umgebungsvariablen
         
# 9. Aufzeichnung von Ereignissen in der API
- Aufzeichnung aller Zugriffe auf die API
           
# 10. Verschlüsselung der Benutzerdaten
- Verwendung eienes starken AES Algorithmus
                 

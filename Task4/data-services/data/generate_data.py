import pandas as pd
import random
from datetime import datetime, timedelta

# ---------- Телеметрия ----------
telemetry = []
now = datetime.utcnow()

for i in range(1, 6):
    for day in range(5):
        telemetry.append({
            "client_id": i,
            "event_time": now - timedelta(days=day),
            "steps": random.randint(1000, 15000),
            "battery_level": random.randint(10, 100)
        })

df_telemetry = pd.DataFrame(telemetry)
df_telemetry.to_csv("data/clients-telemetry-data.csv", index=False)

print("CSV files generated successfully.")
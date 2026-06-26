# Frisbee controller - Pico W (MicroPython) -- WIRELESS over WiFi/UDP
# -------------------------------------------------------------------
# Reads your joystick + accelerometer, detects a jump, and streams a small
# UDP packet to the PC every loop. Unity listens on a socket (see the C#
# file) and jumps when the jump counter increases.
#
# Save on the Pico as  main.py.
#
# Packet format (one UTF-8 line):
#   seq,jump_count,joyX,joyY,joyBtn,gx,gy,gz
#   - jump_count increments by 1 each detected jump (robust to UDP loss)
#   - joyX/Y/Btn and gx/gy/gz are there for the joystick + throw later

import time
import machine
import network
import socket
import math
from machine import Pin, I2C

# -------------------------------------------------------
# WIFI SETUP
# -------------------------------------------------------
wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("tufts_eecs", "foundedin1883")

print("Connecting to WiFi...")
while not wlan.isconnected():
    time.sleep(0.1)
print("WiFi connected:", wlan.ifconfig())

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
dest = (PC_IP, PC_PORT)
PC_IP   = "192.168.1.50"   # <-- your PC's actual IP (Windows: ipconfig, Mac: ifconfig)
PC_PORT = 5005             # must match listenPort in the Unity script

# ============================ sensors (your code) =======================
i2c = I2C(scl=Pin(1), sda=Pin(0), freq=100000)
time.sleep(0.5)
print("I2C devices:", [hex(d) for d in i2c.scan()])


def joystick():
    try:
        x = i2c.readfrom_mem(0x20, 0x03, 2)
        data_x = (x[0] << 8 | x[1]) >> 6
        y = i2c.readfrom_mem(0x20, 0x05, 2)
        data_y = (y[0] << 8 | y[1]) >> 6
        b = i2c.readfrom_mem(0x20, 0x07, 1)
        return (data_x, data_y, b[0])
    except OSError:
        return None


ACCEL_RANGE_CFG    = 0x00       # +/-2g  (use 0x02 for +/-8g if jumps clip)
ACCEL_COUNTS_PER_G = 1024       # 256 if you switch to +/-8g


def setup_accel():
    i2c.writeto_mem(0x1D, 0x2A, bytes([0x00]))
    i2c.writeto_mem(0x1D, 0x0E, bytes([ACCEL_RANGE_CFG]))
    i2c.writeto_mem(0x1D, 0x2A, bytes([0x01]))


def _to_signed_12bit(value):
    return value - 4096 if value > 2047 else value


def read_accel():
    try:
        d = i2c.readfrom_mem(0x1D, 0x01, 6)
        x = _to_signed_12bit((d[0] << 8 | d[1]) >> 4)
        y = _to_signed_12bit((d[2] << 8 | d[3]) >> 4)
        z = _to_signed_12bit((d[4] << 8 | d[5]) >> 4)
        return (x / ACCEL_COUNTS_PER_G, y / ACCEL_COUNTS_PER_G, z / ACCEL_COUNTS_PER_G)
    except OSError:
        return None


setup_accel()      # NOTE: call ONCE here, not inside the loop (your old code re-ran
                   # it every iteration, which needlessly re-initializes the sensor)

# ============================ jump detection ============================
JUMP_G           = 1.7
JUMP_COOLDOWN_MS = 450

jump_count = 0
last_jump  = time.ticks_add(time.ticks_ms(), -1000)
seq        = 0

while True:
    now = time.ticks_ms()

    j = joystick()
    jx, jy, jb = j if j is not None else (0, 0, 0)

    a = read_accel()
    if a is not None:
        gx, gy, gz = a
        mag = math.sqrt(gx * gx + gy * gy + gz * gz)
        if mag > JUMP_G and time.ticks_diff(now, last_jump) > JUMP_COOLDOWN_MS:
            last_jump = now
            jump_count += 1
    else:
        gx = gy = gz = 0.0

    seq += 1
    msg = "{},{},{},{},{},{:.2f},{:.2f},{:.2f}".format(
        seq, jump_count, jx, jy, jb, gx, gy, gz)
    try:
        sock.sendto(msg.encode(), dest)
    except OSError:
        pass

    time.sleep_ms(10)      # ~100 Hz
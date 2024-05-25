import json


class Position:
    def __init__(self, x, y):
        self.x = x
        self.y = y


class Message:
    def __init__(self, json_string: str = "{}"):
        try:
            self.msg = json.loads(json_string)
        except json.JSONDecodeError:
            self.msg = {}

    def json(self):
        try:
            return json.dumps(self.msg)
        except Exception:
            return "{}"


class PerformAbandonMessage(Message):
    def __init__(self, numb: int, token: str, target_supply: str):
        super().__init__()
        self.msg["messageType"] = "PERFORM_ABANDON"
        self.msg["numb"] = numb
        self.msg["token"] = token
        self.msg["targetSupply"] = target_supply


class PerformPickUpMessage(Message):
    def __init__(self, token: str, target_supply: str, num: int):
        super().__init__()
        self.msg["messageType"] = "PERFORM_PICK_UP"
        self.msg["token"] = token
        self.msg["targetSupply"] = target_supply
        self.msg["num"] = num


class PerformSwitchArmMessage(Message):
    def __init__(self, token: str, target_firearm: str):
        super().__init__()
        self.msg["messageType"] = "PERFORM_SWITCH_ARM"
        self.msg["token"] = token
        self.msg["targetFirearm"] = target_firearm


class PerformUseMedicineMessage(Message):
    def __init__(self, token: str, medicine_name: str):
        super().__init__()
        self.msg["messageType"] = "PERFORM_USE_MEDICINE"
        self.msg["token"] = token
        self.msg["medicineName"] = medicine_name


class PerformUseGrenadeMessage(Message):
    def __init__(self, token: str, target_position: Position):
        super().__init__()
        self.msg["messageType"] = "PERFORM_USE_GRENADE"
        self.msg["token"] = token
        self.msg["targetPosition"] = {"x": target_position.x, "y": target_position.y}


class PerformMoveMessage(Message):
    def __init__(self, token: str, destination: Position):
        super().__init__()
        self.msg["messageType"] = "PERFORM_MOVE"
        self.msg["token"] = token
        self.msg["destination"] = {"x": destination.x, "y": destination.y}


class PerformStopMessage(Message):
    def __init__(self, token: str):
        super().__init__()
        self.msg["messageType"] = "PERFORM_STOP"
        self.msg["token"] = token


class PerformAttackMessage(Message):
    def __init__(self, token: str, target_position: Position):
        super().__init__()
        self.msg["messageType"] = "PERFORM_ATTACK"
        self.msg["token"] = token
        self.msg["targetPosition"] = {"x": target_position.x, "y": target_position.y}


class GetPlayerInfoMessage(Message):
    def __init__(self, token: str):
        super().__init__()
        self.msg["messageType"] = "GET_PLAYER_INFO"
        self.msg["token"] = token


class GetMapMessage(Message):
    def __init__(self, token: str):
        super().__init__()
        self.msg["messageType"] = "GET_MAP"
        self.msg["token"] = token


class ChooseOriginMessage(Message):
    def __init__(self, token: str, origin_position: Position):
        super().__init__()
        self.msg["messageType"] = "CHOOSE_ORIGIN"
        self.msg["token"] = token
        self.msg["originPosition"] = {"x": origin_position.x, "y": origin_position.y}


class GrenadeMessage(Message):
    def __init__(self, token: str):
        super().__init__()
        self.msg["messageType"] = "GRENADE_MESSAGE"
        self.msg["token"] = token

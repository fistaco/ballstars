import re
import sys

import pandas as pd

BADMINTON_STRINGS = []
BASKETBALL_STRINGS = []
FLOORBALL_STRINGS = []
KORFBALL_STRINGS = []
SQUASH_STRINGS = []
TABLE_TENNIS_STRINGS = []
VOLLEYBALL_STRINGS = []

SPORT_REGEX_PATTERNS = {
    "Badminton": ["badminton", "(sb\s)?helios"],
    "Basketball": ["basket\s?ball?", "usbf"],
    "Floorball": ["floor\s?ball?", "(usfv\s)?jungle\s?speed"],
    "Korfball": ["korf\s?ball?", "(uskv\s)?hebbes"],
    "Squash": ["squash", "(us\s)?beat\s?it"],
    "Table tennis": ["table\s?tennis", "tafel\s?tennis", "(ust\s)?traiectum"],
    "Volleyball": ["volley\s?ball?", "van slag"]
}


def parse_sport(s):
    # For each sport, check if the given string s contains one of the sport's
    # string patterns.
    for (sport, regex_patterns) in SPORT_REGEX_PATTERNS.items():
        for regex in regex_patterns:
            if re.search(regex, s, re.IGNORECASE):
                return sport


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python3 parse_sport_strings.py player_csv_input")
        exit()

    # Load the data frame from the player CSV file
    filename = sys.argv[1]
    df = pd.read_csv(filename, sep=",")

    # Find the SSV column name
    ssv_colname = next(
        (col for col in df.columns if "SSV" in col or "ssv" in col)
    )

    # Replace the fields in the SSV column with standardised sports names
    df["sport"] = df[ssv_colname].apply(parse_sport)
    df = df.drop(ssv_colname, axis=1)

    # Write the DF with the converted sports names to a new file
    base_filename = filename.strip(".csv")
    output_filename = f"{base_filename}-converted-sport-names.csv"
    df.to_csv(output_filename, index=False)
    print(f"CSV with converted sport names written to {output_filename}")

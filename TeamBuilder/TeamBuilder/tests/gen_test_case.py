import random as rnd
import sys

import numpy as np
import pandas as pd


# Define values with which we will synthesize player data
GENDERS = ["Female", "Male"]
FIRST_NAMES = {
    "Female": "Alice",
    "Male": "Bob"
}
SSV_NAMES = ["UST Traiectum", "USKV Hebbes", "USBF", "SB Helios",
             "USFV Jungle Speed"]


def random_player_gen(df, player_count):
    for i in range(player_count):
        # Generate a random gender and SSV
        gender = rnd.choice(GENDERS)
        rnd_ssv = rnd.choice(SSV_NAMES)

        # Set DF values according to the randomly chosen geder and ssnv
        set_row_vals(df, i, gender, rnd_ssv)


def balanced_player_gen(df, player_count):
    # Balance each SSV's member count and the male/female ratio.
    players_per_gender = player_count//2
    players_per_ssv = player_count//len(SSV_NAMES)
    print(f"Generating {player_count} players ({players_per_ssv} per SSV, {players_per_gender} of each gender)")

    for (i, ssv) in enumerate(SSV_NAMES):
        for (j, gender) in enumerate(GENDERS):
            for k in range(players_per_ssv//2):
                index = i*players_per_ssv + j*players_per_ssv//2 + k
                print(index)
                set_row_vals(df, index, gender, ssv)


def set_row_vals(df, row_nr, gender, ssv):
    # Set names according to the randomly chosen gender
    df.loc[row_nr, "First name"] = FIRST_NAMES[gender]
    df.loc[row_nr, "Last name"] = gender[0]  # First letter of the gender
    df.loc[row_nr, "Sex"] = gender

    # Choose a random SSV
    df.loc[row_nr, "Which SSV are you member of?"] = ssv


if __name__ == "__main__":
    if len(sys.argv) not in [2, 3]:
        print("Usage: python3 gen_test_case.py player_amount [balanced_dist]")
        exit()

    player_count = int(sys.argv[1])  # Take the desired player count as an arg
    balanced_dist = sys.argv[2] if len(sys.argv) == 3 else False

    # Define columns to fill and set up a dataframe
    cols = ["First name", "Last name", "Sex", "Which SSV are you member of?"]
    df = pd.DataFrame(
        np.empty((player_count, len(cols)), dtype=str),
        columns=cols
    )

    if not balanced_dist:
        random_player_gen(df, player_count)
    else:
        balanced_player_gen(df, player_count)

    # Save to CSV
    filename = f"./test-cases/test-input-{player_count}-players"
    if balanced_dist:
        filename += "-balanced"
    filename += ".csv"
    df.to_csv(filename, index=False)
    print(f"Test case saved to {filename}.")

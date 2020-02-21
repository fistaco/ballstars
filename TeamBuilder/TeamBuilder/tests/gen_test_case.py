import random as rnd
import sys

import numpy as np
import pandas as pd

if __name__ == "__main__":
    player_count = int(sys.argv[1])  # Take the desired player count as an arg

    # Define columns to fill and set up a dataframe
    cols = ["First name", "Last name", "Sex", "Which SSV are you member of?"]
    df = pd.DataFrame(
        np.empty((player_count, len(cols)), dtype=str),
        columns=cols
    )

    # Define values with which we will synthesize player data
    genders = ["Female", "Male"]
    first_names = {
        "Female": "Alice",
        "Male": "Bob"
    }
    ssv_names = ["UST Traiectum", "USKV Hebbes", "USBF", "SB Helios",
                 "USFV Jungle Speed"]

    for i in range(player_count):
        gender = rnd.choice(genders)

        # Set names according to the randomly chosen gender
        df.loc[i, "First name"] = first_names[gender]
        df.loc[i, "Last name"] = gender[0]  # First letter of the chosen gender
        df.loc[i, "Sex"] = gender

        # Choose a random SSV
        df.loc[i, "Which SSV are you member of?"] = rnd.choice(ssv_names)

    # Save to CSV
    filename = f"./test-input-{player_count}-players.csv"
    df.to_csv(filename, index=False)
    print(f"Test case saved to {filename}.")

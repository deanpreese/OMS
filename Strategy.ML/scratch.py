from itertools import combinations

# Define a set of 13 items
items = {'Item1', 'Item2', 'Item3', 'Item4', 'Item5', 'Item6', 'Item7', 'Item8', 'Item9', 'Item10', 'Item11', 'Item12', 'Item13'}

# Generate all 5-item combinations from the set of 13 items
five_item_combinations = list(combinations(items, 2))

# Print out all 5-item combinations
for combo in five_item_combinations:
    print(combo)

# Optionally, to see the total number of combinations
print(f"Total combinations: {len(five_item_combinations)}")
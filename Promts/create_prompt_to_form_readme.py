import os
import re # Import regex for cleaning filenames
import sys # To check python version for f-strings potentially

# --- Configuration ---
src_dirs = [r"d:\projects\dev-my-sandbox\zlogger-sandbox\Assets\Example.03 - FileProvider"] # Use raw string for Windows paths
file_extensions = [".cs"]  # Array of file extensions to include
name_of_examples = "File Provider Examples" # Descriptive name for examples

# --- Generate dynamic output filename ---
# 1. Create a filename-safe version (lowercase, replace non-alphanumeric with '_')
safe_name = re.sub(r'\W+', '_', name_of_examples).lower()
# 2. Construct the output filename using the safe name
output_txt_file = f"{safe_name}_make-readme-prompt.txt"

# --- Base prompt template ---
# Uses placeholders for dynamic content ({name_of_examples}, {output_filename})
# This template will be formatted *after* the output filename is determined.
base_prompt_template = """

-----------------------------------------------------
PROMPT:
-----------------------------------------------------
- Review the source code provided. The examples are about: {name_of_examples} in ZLogger library.
- Create a short readme.md for these examples.
- in intro section add some short info on {name_of_examples} in ZLogger
- ## Header: Use the common name: `# {name_of_examples}`
- ## Features Demonstrated: Create a bulleted list summarizing the key features or techniques shown *only* in the provided code snippets. Focus on what the examples illustrate.
- ##Summary: Write a brief concluding paragraph summarizing the purpose and content of the examples. Focus on particular features used in the code.
- The final readme.md content should be a clean markdown text, free of any internal references or citations. Do not include any bracketed numbers or similar citation markers.
- Ensure the readme is based *solely* on the code included in '{output_filename}'.

Give me 2-3 variants of the readme.
"""

# --- Function Definition ---
def collect_source_to_txt(dirs, exts, output_file, final_prompt_str):
    """
    Collects source code files with specified extensions from specified directories
    into a single text file, then appends a final prompt.

    Args:
        dirs (list): List of directories to analyze.
        exts (list): List of file extensions to include.
        output_file (str): The path to the output text file.
        final_prompt_str (str): The prompt string to append at the end.

    Returns:
        bool: True if at least one file was successfully collected, False otherwise.
    """
    print("-" * 50)
    print(f"Starting source code collection...")
    print(f"Source Directories: {dirs}")
    print(f"File Extensions: {exts}")
    print(f"Output File: {output_file}")
    print("-" * 50)

    files_collected_count = 0

    try:
        # Open the output file (overwrite if exists)
        with open(output_file, 'w', encoding='utf-8') as outfile:
            # Add a header to the blob file
            outfile.write(f"// Source Code Blob for: {name_of_examples}\n")
            outfile.write(f"// Generated from directories: {', '.join(dirs)}\n")
            outfile.write(f"// File Extensions: {', '.join(exts)}\n")
            outfile.write("-" * 50 + "\n\n")

            for root_dir in dirs:
                print(f"Processing directory: '{root_dir}'")
                # Check if directory exists
                if os.path.exists(root_dir) and os.path.isdir(root_dir):
                    files_found_in_dir = 0
                    # Walk through the directory tree
                    for subdir, _, files in os.walk(root_dir):
                        for file in files:
                            # Check if the file has one of the desired extensions
                            if any(file.endswith(ext) for ext in exts):
                                filepath = os.path.join(subdir, file)
                                # Use relative path for cleaner comments in the output file
                                relative_filepath = os.path.relpath(filepath, start=root_dir)
                                print(f"  (+) Collecting: {relative_filepath}")
                                try:
                                    # Read the source file
                                    with open(filepath, 'r', encoding='utf-8') as infile:
                                        # Write a separator and the file content
                                        outfile.write(f"// --- Start File: {relative_filepath} ---\n\n")
                                        outfile.write(infile.read())
                                        outfile.write(f"\n\n// --- End File: {relative_filepath} ---\n\n")
                                    files_found_in_dir += 1
                                except Exception as e:
                                    error_msg = f"  (!) ERROR reading file '{filepath}': {e}"
                                    print(error_msg)
                                    outfile.write(f"// !!! Error reading file {relative_filepath}: {e} !!!\n\n")

                    if files_found_in_dir > 0:
                         print(f"  -> Collected {files_found_in_dir} file(s) from '{root_dir}'.")
                         files_collected_count += files_found_in_dir
                    else:
                         print(f"  -> No files with specified extensions found in '{root_dir}'.")

                else:
                    print(f"(!) WARNING: Directory '{root_dir}' does not exist or is not a directory. Skipping.")

            # Append the final prompt after all files
            print(f"\nAppending final prompt to '{output_file}'...")
            outfile.write(final_prompt_str)
            print("Prompt appended successfully.")

    except IOError as e:
         print(f"\n(!) FATAL ERROR: Could not write to output file '{output_file}': {e}")
         return False # Indicate failure
    except Exception as e:
         print(f"\n(!) FATAL ERROR: An unexpected error occurred: {e}")
         return False # Indicate failure

    print("-" * 50)
    # Return True only if at least one file was actually processed
    return files_collected_count > 0

# --- Main Execution Block ---
if __name__ == "__main__":
    print("Script execution started.")

    # Format the final prompt string using the dynamic filename and example name
    final_prompt = base_prompt_template.format(
        output_filename=output_txt_file,
        name_of_examples=name_of_examples
    )

    # Call the collection function, passing the final formatted prompt
    success = collect_source_to_txt(src_dirs, file_extensions, output_txt_file, final_prompt)

    # Final status message based on whether files were collected
    print("-" * 50)
    if success:
        print(f"✅ Source code successfully collected into: {output_txt_file}")
    else:
        print(f"⚠️ Source code collection finished, but no matching files were found or an error occurred.")
        print(f"   Please check the output file (if created) and console logs: {output_txt_file}")
    print("-" * 50)
    print("Script execution finished.")
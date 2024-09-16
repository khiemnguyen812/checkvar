import pandas as pd
import sys
import io
import json
import re

def read_csv_fast(file_path, keyword):
    usecols = ['DocNo', 'Debit', 'Credit', 'Balance', 'TransactionsInDetail']
    dtype = {
        'DocNo': str,
        'Debit': float,
        'Credit': float,
        'Balance': float,
        'TransactionsInDetail': str
    }

    chunksize = 10000
    keyword_lower = keyword.lower()
    results = []
    
    keyword_pattern = re.compile(r'\b' + re.escape(keyword_lower) + r'\b')

    for chunk in pd.read_csv(file_path, usecols=usecols, dtype=dtype, chunksize=chunksize):
        matching_rows = chunk[chunk.apply(lambda row: keyword_pattern.search(' '.join(row.astype(str)).lower()) is not None, axis=1)]
        for index, row in matching_rows.iterrows():
            row_dict = {col: dtype[col](row[col]) if pd.notna(row[col]) else None for col in usecols}
            results.append(row_dict)
    
    return results

if __name__ == '__main__':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')  # Thay đổi encoding của stdout thành UTF-8
    file_path = sys.argv[1]
    keyword = sys.argv[2]
    results = read_csv_fast(file_path, keyword)
    if isinstance(results, list) and all(isinstance(item, dict) for item in results):
        json_data = json.dumps(results, ensure_ascii=False, indent=4)
        print(json_data)
    else:
        print("Error: The output is not a valid list of dictionaries.")
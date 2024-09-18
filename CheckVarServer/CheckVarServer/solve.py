import pandas as pd
import re
import json
import sys
import io
from concurrent.futures import ThreadPoolExecutor

def process_chunk(chunk, keyword_pattern, usecols, dtype):
    matching_rows = chunk[chunk.apply(lambda row: keyword_pattern.search(' '.join(row.astype(str)).lower()) is not None, axis=1)]
    results = []
    for index, row in matching_rows.iterrows():
        row_dict = {col: dtype[col](row[col]) if pd.notna(row[col]) else None for col in usecols}
        results.append(row_dict)
    return results

def read_csv_fast(file_path, keyword, num_threads=None):
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
    keyword_pattern = re.compile(r'\b' + re.escape(keyword_lower) + r'\b')
    results = []

    with ThreadPoolExecutor(max_workers=num_threads) as executor:
        futures = []
        for chunk in pd.read_csv(file_path, usecols=usecols, dtype=dtype, chunksize=chunksize):
            futures.append(executor.submit(process_chunk, chunk, keyword_pattern, usecols, dtype))
        
        for future in futures:
            results.extend(future.result())

    return results

if __name__ == '__main__':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    file_path = sys.argv[1]
    keyword = sys.argv[2]
    results = read_csv_fast(file_path, keyword,num_threads=100)
    if isinstance(results, list) and all(isinstance(item, dict) for item in results):
        json_data = json.dumps(results, ensure_ascii=False, indent=4)
        print(json_data)
    else:
        print("Error: The output is not a valid list of dictionaries.")
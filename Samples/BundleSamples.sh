#!/bin/bash

# Directory, where is placed the SingleHtmlAppBundler.dll application file
AppDir="/home/someuser/SingleHtmlAppBundler/Bin/"

# Directory containing the samples
SamplesDir="/home/someuser/SingleHtmlAppBundler/Samples/"

# The Minify variable, set "1" for disable all minifications
Minify="0"

# Clear Bundled directory
rm -r ${SamplesDir}Bundled/*

# Bundle Multimedia sample
dotnet ${AppDir}SingleHtmlAppBundler.dll \
 ${SamplesDir}Multimedia/index.html \
 ${SamplesDir}Bundled/ \
 BundleHtmlBody=1 BundleHtmlScript=1 BundleHtmlLink=1 \
 BundleHtmlIframe=1 BundleHtmlImg=1 BundleHtmlAudio=1 BundleHtmlVideo=1 \
 BundleHtmlTrack=1 BundleHtmlSource=1 BundleHtmlObject=1 BundleHtmlEmbed=1 \
 BundleJsUrl=1 BundleJsWorker=1 BundleJsSharedWorker=1 BundleJsFetch=1 BundleJsImportScripts=1 \
 MinifyHtmlComment=${Minify} MinifyHtmlWhitespace=${Minify} MinifyJsComment=${Minify} MinifyJsWhitespace=${Minify}

# Rename and move bundled file into output directory
mv ${SamplesDir}Bundled/index.html ${SamplesDir}Bundled/multimedia_${Minify}.html

# WebAssembly sample compilation commands with Emscripten - run these commands being inside WebAssembly subdirectory
#rm -r *.wasm
#rm -r *.js
#rm -r *.html
#emcc wasmtest.cpp -o wasmtest0.html -s "EXPORTED_RUNTIME_METHODS=['ccall','cwrap']"
#emcc wasmtest.cpp -o wasmtest1.html -s "EXPORTED_RUNTIME_METHODS=['ccall','cwrap']" -pthread -s PTHREAD_POOL_SIZE=5

# Bundle WebAssembly sample without threads
dotnet ${AppDir}SingleHtmlAppBundler.dll \
 ${SamplesDir}WebAssembly/wasmtest0.html \
 ${SamplesDir}Bundled/ \
 CodePreparationFile=${SamplesDir}PrepareWasm1.txt \
 BundleHtmlScript=1 BundleJsWorker=1 BundleJsFetch=1 \
 MinifyHtmlComment=${Minify} MinifyHtmlWhitespace=${Minify} MinifyJsComment=${Minify} MinifyJsWhitespace=${Minify}

# Rename and move bundled file into output directory
mv ${SamplesDir}Bundled/wasmtest0.html ${SamplesDir}Bundled/wasmtest0_${Minify}.html

# Bundle WebAssembly sample with threads - step 1/2 - prepare "worker" script
dotnet ${AppDir}SingleHtmlAppBundler.dll \
 ${SamplesDir}WebAssembly/wasmtest1.worker.js \
 ${SamplesDir}Bundled/ \
 CodePreparationFile=${SamplesDir}PrepareWasm2.txt \
 BundleJsImportScripts=1 \
 MinifyHtmlComment=${Minify} MinifyHtmlWhitespace=${Minify} MinifyJsComment=${Minify} MinifyJsWhitespace=${Minify}

# Rename and move bundled file into input directory
mv ${SamplesDir}Bundled/wasmtest1.worker.js ${SamplesDir}WebAssembly/_bundled_wasmtest1.worker.js

# Bundle WebAssembly sample with threads - step 2/2 - prepare "worker" script
dotnet ${AppDir}SingleHtmlAppBundler.dll \
 ${SamplesDir}WebAssembly/wasmtest1.html \
 ${SamplesDir}Bundled/ \
 CodePreparationFile=${SamplesDir}PrepareWasm3.txt \
 BundleHtmlScript=1 BundleJsWorker=1 BundleJsFetch=1 \
 MinifyHtmlComment=${Minify} MinifyHtmlWhitespace=${Minify} MinifyJsComment=${Minify} MinifyJsWhitespace=${Minify}

# Remove temporary worker file from input directory
rm ${SamplesDir}WebAssembly/_bundled_wasmtest1.worker.js

# Rename and move bundled file into output directory
mv ${SamplesDir}Bundled/wasmtest1.html ${SamplesDir}Bundled/wasmtest1_${Minify}.php

read -p "Press Enter to exit"


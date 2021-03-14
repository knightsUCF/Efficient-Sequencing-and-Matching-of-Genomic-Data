
# Efficient Sequencing of Genomic Data


<h2> Burrows-Wheeler Transform </h2>

Sequencing of genomic data requires large processing considerations with characters up to 4 billion per a homo sapiens sequence. A number of methods have been proposed to optimize the storing and processing of data. One of these is the Burrows-Wheeler algorithm.

```Python

# modified from: https://github.com/egonelbre/fm-index

class Sequencer:


    def __init__(self, sequence):
        self.sequence = sequence
        self.EOS = "\0"


    # get Burrows-Wheeler transform
    
    def make_transform(self):
        self.sequence += self.EOS
        rotations = [self.sequence[i:] + self.sequence[:i] for i in range(len(self.sequence))]
        table = sorted(rotations)
        last_column = [row[-1] for row in table]
        r = "".join(last_column)
        return r


    def get_transform(self, s):
        table = [""] * len(s)
        for i in range(len(s)):
            prepended = [s[i] + table[i] for i in range(len(s))]
            table = sorted(prepended)
        for row in table:
            if row.endswith(self.EOS):
                s = row
                break
        s = s.rstrip(self.EOS)
        return s
 ```
 
 
 <h3> Implementation of Burrows-Wheeler Transform </h3>
 
 
 ```Python
 
sequence = 'TCAGATCAGTC'https://github.com/knightsUCF/GSFolder/blob/main/research/FMIndex/Images/FM%20Index.png
sequencer = Sequencer(sequence)

sequencer.make_transform())
# output: CCCGTTTAAGA

print(sequencer.get_transform(sequence))
# output: TCAGATCAGTC

```

From the output we notice that similar characters are grouped together: CCCGTTTAAGA

<h4>Why is the Burrows-Wheeler Transform useful?</h4>
 
Because the transform provides a way of grouping similar characters together, we can store the counts of grouped characters, saving space, and allow for decoding back to the original sequence. This way we can compress large sequences of data.

<h4>How does the decoding process work?</h4>

1) make empty table for the suffix array

2) use lf-mapping to reverse the tranformation

3) add one letter for each partial string in the suffix array

4) convert to sorted suffix array

5) find the correct row (ending with the end of line character)

source: https://github.com/egonelbre/fm-index/blob/master/src/bwt.py

<h4>What is "LF mapping"?</h4>

"LF(r) is a function mapping from the last column of the BWT to the first column. Parameter r is the row number. In the last column of row r in the BWT matrix, say we have the ith occurrence of character j."

source: https://web.stanford.edu/class/cs262/archives/notes/lecture4.pdf

<h2> FM-Index </h2>

The FM-Index is a combination of the Burrows-Wheeler Transform and other data structures. The central structure of the FM Index consists of the first and last columns of the Burrows-Wheeler rotated permutations matrix. Only these two columns are stored in the FM Index. The matrix data between the two columns can be discarded for compression efficiency.


```
A X X X X X A
A X X X X X A
A X X X X X A
A X X X X X C
C X X X X X C
C X X X X X T
C X X X X X T
```

<h4>How to query the FM Index?</h4>

We search for a prefix pattern among the columns of the FM Index. Similar to a binary tree we can search for the first part of the prefix, and then filter down to further detailed prefixes which are contained within. Since the left column of the Burrows Wheeler matrix will be ordered, and this is part of our FM Index data structure, the search will be ordered among similar context.

By searching the rest of the suffix on the other column of the FM Index data, even though we don't have the structure in the middle, we can tell which letter preceeded the first column, and hence is part of the sequence. Then we can repeat the process to further filter down into the relevant range of the queried sequence. This is why the FM Index is useful, because we can do a fast sequentially relevant search.

![](https://github.com/knightsUCF/GSFolder/blob/main/research/FMIndex/Images/FM%20Index.png)

reference: https://www.youtube.com/watch?v=kvVGj5V65io

<h4>Example Code: Searching the FM Index</h4>

```Python

def _walk(self, idx):
        """ find the offset in position idx of transformed string
            from the beginning """
        
        # walk to the beginning using lf mapping
        # this is same as inverse of burrow wheeler transformation
        # from arbitrary location
        r = 0
        i = idx 
        while self.data[i] != bw.EOS:
            if self.offset.get(i):
                # we have cached the location and can use it
                r += self.offset[i]
                break
            r += 1
            i = self._lf(i, self.data[i])
        
        # save the offset of some idx for faster searches
        if not self.offset.get(idx):
            self.offset[i] = r
        return r
        
        
def bounds(self, q):
        """ find the first and last suffix positions for query q """
        top = 0
        bot = len(self.data)
        for i, qc in enumerate(q[::-1]):
            top = self._lf(top, qc)
            bot = self._lf(bot, qc)
            if top == bot: return (-1,-1)
        return (top,bot)
    
    # search the FM Index
    
    def search(self, q):
        """ search the positions of query q """
        
        # find the suffixes for the query
        top, bot = self.bounds(q)
        matches = []
        # find the location of the suffixes
        # by walking the reverse text from that position
        # with lf mapping
        for i in range(top, bot):
            pos = self._walk(i)
            matches.append(pos)
        return sorted(matches)

# source: https://github.com/egonelbre/fm-index/blob/master/src/fmindex.py

```

<h4>Determining where the occurances are</h4>

We have the range of our data and now we need the location. To prevent scanning the last row linearly, and be more efficient, we can
precalculate how many times the letter has already occured up to that row. Then we can use this as a lookup table to speed up the search. The lookup table will give us the range. We can further make a space optimization by only including every few rows in the lookup table. We can also create a lookup table for the suffix array, and use that rto determine our relative row location.

<h2> Using short reads to correct long reads </h2>

Now that we have defined a way to compress large data (Burrows-Wheeler Transform), and store the data in a manner to be queried sequentialy relevant (FM Index), we come to FMLRC proposed by the paper, "FMLRC: Hybrid long read error correction using an FM-index"

https://bmcbioinformatics.biomedcentral.com/articles/10.1186/s12859-018-2051-3

"Given a BWT of the short-read sequencing data, FMLRC will build an FM-index and use that as an implicit de Bruijn graph."

source: https://github.com/holtjma/fmlrc

<h4>Summary of the strategy</h4>

"Given a BWT of the short-read sequencing data, FMLRC will build an FM-index and use that as an implicit de Bruijn graph. Each long read is then corrected independently by identifying low frequency k-mers in the long read and replacing them with the closest matching high frequency k-mers in the implicit de Bruijn graph. In contrast to other de Bruijn graph based implementations, FMLRC is not restricted to a particular k-mer size and instead uses a two pass method with both a short "k-mer" and a longer "K-mer". This allows FMLRC to correct through low complexity regions that are computational difficult for short k-mers."

<h4>Definitions</h4>
    
<h5>K-Mers</h5>

We define k-mers, as all possible sequentially overlapping strings of length "k".

For example given a sequence: A G T C C A, k-mers of length three would be: AGT, GTC, TCC, CCA.

K-mers are particularly relevant for our purposes because when aligning genomic sequences we do not exactly know where the sequence begins or ends, when there are mutations and variations between organisms. Instead we try to find the "best possible fit".

However if were were to scan the entire sequence with a static size k-mer that would be computationally slow compared to what FMLRC proposes: a way to dynamically scan k-mers."

<h5>De Bruijn Graphs</h5>

De Bruijn graphs are representations of overlapping sequences. "FMLRC will build an FM-index and use that as an implicit de Bruijn graph."

The paper mentions that they are using the FM Index implicitly as a De Bruijn graph, since both data structures imply a sequential order.

<h4>FM Index Construction</h4>

The FM Index, which serves as an implicit de Bruijn graph can be found in the rle_bwt.cpp file. Here we can see that we are using the Burrows-Wheeler methadology stated earlier, and also we are creating an offset to sequentially order the data for fast lookup queries.

```cpp
void RLE_BWT::constructFMIndex() {
    //figure out the number of entries and pre-allocate
    //uint64_t samplingSize = (uint64_t)ceil(((float)this->totalSize+1)/this->binSize);
    uint64_t samplingSize = (uint64_t)ceil(((float)this->totalSize+1)/this->binSize)+1;
    this->fmIndex = new uint64_t*[VC_LEN];
    for(int x = 0; x < VC_LEN; x++) {
        this->fmIndex[x] = new uint64_t[samplingSize];
    }
    this->refFM = vector<uint64_t>(samplingSize, 0);
    
    uint8_t prevChar = 0;
    uint64_t totalCharCount = 0;
    uint64_t powerMultiple = 1;
    uint64_t binEnd = 0;
    uint64_t binID = 0;
    uint64_t bwtIndex = 0;
    uint64_t prevStart = 0;
    uint8_t currentChar;
    
    vector<uint64_t> countsSoFar = vector<uint64_t>(VC_LEN);
    for(int x = 0; x < VC_LEN; x++) {
        countsSoFar[x] = this->startIndex[x];
    }
    
    //go through each run in the BWT and set FM-indices as we go
    uint64_t numBytes = this->bwt.size();
    for(uint64_t x = 0; x < numBytes; x++) {
        currentChar = this->bwt[x] & MASK;
        if(currentChar == prevChar) {
            totalCharCount += (this->bwt[x] >> LETTER_BITS) * powerMultiple;
            powerMultiple *= NUM_POWER;
        }
        else {
            //first save the current FM-index entry
            while(bwtIndex+totalCharCount >= binEnd) {
                this->refFM[binID] = prevStart;
                for(int y = 0; y < VC_LEN; y++) {
                    this->fmIndex[y][binID] = countsSoFar[y];
                }
                binEnd += this->binSize;
                binID++;
            }
            
            //now add the previous
            countsSoFar[prevChar] += totalCharCount;
            bwtIndex += totalCharCount;
            
            prevChar = currentChar;
            prevStart = x;
            totalCharCount = this->bwt[x] >> LETTER_BITS;
            powerMultiple = NUM_POWER;
        }
    }
    
    while(bwtIndex+totalCharCount >= binEnd) {
        this->refFM[binID] = prevStart;
        for(int y = 0; y < VC_LEN; y++) {
            this->fmIndex[y][binID] = countsSoFar[y];
        }
        binEnd += this->binSize;
        binID++;
    }
    
    //set the last entry
    countsSoFar[prevChar] += totalCharCount;//forces countSoFar to hold the very end FM-index entry
    this->refFM[samplingSize-1] = numBytes; //need to point to the index at the end
    for(int y = 0; y < VC_LEN; y++) {
        this->fmIndex[y][samplingSize-1] = countsSoFar[y];
    }
    
    //calculate the total offsetSum
    this->offsetSum = 0;
    for(int x = 0; x < VC_LEN; x++) {
        this->offsetSum += this->fmIndex[x][0];
    }
}
```

<h4>Core Processing</h4>

The core processing of the FMLRC strategy takes place in main.cpp. And more specifically the dynamic k-mer matching strategy is found in multiBridge(), one of the main advantages posed by the paper:

```cpp
vector<vector<uint8_t> > multiBridge(BaseBWT * rle_p, vector<uint8_t> seedKmer, vector<uint8_t> targetKmer, uint64_t tMin, uint64_t branchLim, uint64_t maxBranchLen) {
    /*
    printf("multibridge ");
    for(int x = 0; x < seedKmer.size(); x++) printf("%d", seedKmer[x]);
    printf(" ");
    for(int x = 0; x < targetKmer.size(); x++) printf("%d", targetKmer[x]);
    printf("\n");
    */
    
    //printf("tMin = %d\n", tMin);
    
    vector<vector<uint8_t> > ret = vector<vector<uint8_t> >(0);
    uint64_t kmerLen = seedKmer.size();
    
    vector<uint64_t> counts = vector<uint64_t>(4);
    
    uint64_t numBranched = 0;
    
    // cdef str currBridge
    vector<uint8_t> currBridge;
    // cdef list currBridgeList
    uint64_t currBridgeLen = 0;
    vector<uint8_t> currKmer = vector<uint8_t>(kmerLen, 4);
    vector<uint8_t> revKmer = vector<uint8_t>(kmerLen, 4);
    
    // cdef list possBridges = [(seedKmer, kmerLen)]
    vector<vector<uint8_t> > possBridges = vector<vector<uint8_t> >();
    possBridges.push_back(vector<uint8_t>(seedKmer));
    
    // cdef unsigned char * currBridge_view
    // cdef unsigned char * currKmer_view = <bytes>currKmer
    // cdef unsigned char * revKmer_view = <bytes>revKmer
    // cdef unsigned char * targetKmer_view = <bytes>targetKmer
    // #print currKmer_view[0], ord('A')
    
    // cdef unsigned long i, x
    // cdef str c
    
    uint64_t maxPos;
    
    //while we have things to explore, and we haven't explored too many, and we don't have a ridiculous number of possibilities
    while(possBridges.size() > 0 && numBranched < branchLim) {
        currBridge = possBridges.back();
        possBridges.pop_back();
        currBridgeLen = currBridge.size();
        numBranched++;
        
        for(unsigned int x = 0; x < kmerLen; x++) {
            currKmer[x] = currBridge[currBridgeLen-kmerLen+x];
            revKmer[kmerLen-x-1] = string_util::REV_COMP_I[currKmer[x]];
        }
        
        //try to extend the bridge
        while(currBridgeLen < maxBranchLen) {
            //shift the current k-mer over one in preparation for the last base toggle
            for(unsigned int x = 0; x < kmerLen-1; x++) {
                currKmer[x] = currKmer[x+1];
                revKmer[kmerLen-x-1] = revKmer[kmerLen-x-2];
            }
            
            maxPos = 0;
            
            //count and pick the highest
            //printf("counts ");
            for(int x = 0; x < VALID_CHARS_LEN; x++) {
                currKmer[kmerLen-1] = VALID_CHARS[x];
                revKmer[0] = string_util::REV_COMP_I[VALID_CHARS[x]];
                counts[x] = rle_p->countKmer(&currKmer[0], kmerLen)+rle_p->countKmer(&revKmer[0], kmerLen);
                //printf("%d ", counts[x]);
                if(counts[x] > counts[maxPos]) maxPos = x;
            }
            //printf("\n");
            
            //make sure the highest is high enough for us to consider it
            if(counts[maxPos] >= tMin) {
                currBridge.push_back(4);
                
                if(possBridges.size() < branchLim) {
                    for(unsigned int x = 0; x < VALID_CHARS_LEN; x++) {
                        if(x != maxPos && counts[x] >= tMin) {
                            //add the ones we aren't exploring right now if they're high enough
                            currBridge[currBridgeLen] = VALID_CHARS[x];
                            possBridges.push_back(vector<uint8_t>(currBridge.begin(), currBridge.end()));
                        }
                    }
                }
                else {
                    //printf("exit A\n");
                    return vector<vector<uint8_t> >();
                }
                
                //now really add the symbol
                currBridge[currBridgeLen] = VALID_CHARS[maxPos];
                currBridgeLen++;
                
                currKmer[kmerLen-1] = VALID_CHARS[maxPos];
                revKmer[0] = string_util::REV_COMP_I[VALID_CHARS[maxPos]];
            }
            else {
                //our BEST doesn't pass the threshold on this path, stop following
                //print currBridge, counts, tMin
                break;
            }
            
            if(equal(targetKmer.begin(), targetKmer.end(), currKmer.begin())) {
                ret.push_back(currBridge);
                if(ret.size() >= branchLim) {
                    //printf("exit B\n");
                    return vector<vector<uint8_t> >();
                }
            }
        }
    }
    
    if(numBranched < branchLim) {
        return ret;
    }
    else {
        //printf("exit C\n");
        return vector<vector<uint8_t> >();
    }
}
```

The other 2 major functions found in main.cpp are: (too long to include here)

1) shortAssemble

2) correctionPass

In shortAssemble there is the very important concept introduce of limiting exponential branching. So if we can't find a matching sequence we can limit the pass so the computations don't get stuck in exponential variations.

<h4>Significant Contribution</h4>

Now we come to perhaps the most significant contribution of the method, known as "bridging".

"FMLRC dynamically adjusts these thresholds at run-time for each pass over a long read. A single process will correct the read using the implicit short k-mer de Bruijn graph and then the implicit long K-mer de Bruijn graph". In this way the approach is computationally efficient because we are spending our resources on matching short k-mers to correct the long read, instead of matching an entire long read versus another long read.

```cpp
//try to extend the bridge
        while(currBridgeLen < maxBranchLen) {
            //shift the current k-mer over one in preparation for the last base toggle
            for(unsigned int x = 0; x < kmerLen-1; x++) {
                currKmer[x] = currKmer[x+1];
                revKmer[kmerLen-x-1] = revKmer[kmerLen-x-2];
            }
            
            maxPos = 0;
            
            //count and pick the highest
            //printf("counts ");
            for(int x = 0; x < VALID_CHARS_LEN; x++) {
                currKmer[kmerLen-1] = VALID_CHARS[x];
                revKmer[0] = string_util::REV_COMP_I[VALID_CHARS[x]];
                counts[x] = rle_p->countKmer(&currKmer[0], kmerLen)+rle_p->countKmer(&revKmer[0], kmerLen);
                //printf("%d ", counts[x]);
                if(counts[x] > counts[maxPos]) maxPos = x;
            }
            //printf("\n");
            
            //make sure the highest is high enough for us to consider it
            if(counts[maxPos] >= tMin) {
                currBridge.push_back(4);
                
                if(possBridges.size() < branchLim) {
                    for(unsigned int x = 0; x < VALID_CHARS_LEN; x++) {
                        if(x != maxPos && counts[x] >= tMin) {
                            //add the ones we aren't exploring right now if they're high enough
                            currBridge[currBridgeLen] = VALID_CHARS[x];
                            possBridges.push_back(vector<uint8_t>(currBridge.begin(), currBridge.end()));
                        }
                    }
                }
                else {
                    //printf("exit A\n");
                    return vector<vector<uint8_t> >();
                }
                
                //now really add the symbol
                currBridge[currBridgeLen] = VALID_CHARS[maxPos];
                currBridgeLen++;
                
                currKmer[kmerLen-1] = VALID_CHARS[maxPos];
                revKmer[0] = string_util::REV_COMP_I[VALID_CHARS[maxPos]];
            }
            else {
                //our BEST doesn't pass the threshold on this path, stop following
                //print currBridge, counts, tMin
                break;
            }
        }
 ```

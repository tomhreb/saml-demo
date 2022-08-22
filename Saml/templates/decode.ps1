$request = "jZFBa4NAEIX%2FiuxdV402m0UFExMIpFCStofepmZKBN21O2Na%2ButrDDkVSq4z8x7ve5MRdG2vy4FPZo%2BfAxJ7311rKBeDM9oCNaQNdEiaa30oH3c6DkLdIcMRGIS3rXKxUUmllCrL2TJJVtFSJclsnpbJWsVRWC3WwntFR401uRjFo4ZowK0hBsPjKIxjP1R%2BHD9HSodKp%2FNgkT68Xe6egKg5Yy4%2BoCUUXkmEjkenlTU0dOgO6M5NjS%2F7XS5OzD1pKaHv%2FQuWf8TOBvAzOPzCd2oYKTDIctpBTVJcUfXUwf%2FAvbNsa9sKb2NdjVNft1RFNgG5e4qDG4Ao7o2byat9kcm%2Fzyp%2BAQ%3D%3D"

$urlDecoded = [System.Web.HttpUtility]::UrlDecode($response);

$bytes = [Convert]::FromBase64String($urlDecoded)

$ms = New-Object System.IO.MemoryStream 
$readerStream = New-Object System.IO.MemoryStream -ArgumentList @(,$bytes)
$deflateStream = New-Object System.IO.Compression.DeflateStream($readerStream, [IO.Compression.CompressionMode]::Decompress)
$deflateStream.CopyTo($ms)
$deflateStream.Flush()

$ms.Position = 0
$bytes2 = $ms.ToArray()

$request = [System.Text.Encoding]::UTF8.GetString($bytes2)

$request
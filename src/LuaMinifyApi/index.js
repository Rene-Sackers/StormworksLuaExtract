var http = require('http');
var luamin = require("luamin");

http.createServer(function (req, res) {
	let lua = '';
	req.on('data', chunk => {
		lua += chunk.toString(); // convert Buffer to string
	});
	req.on('end', () => {
		var minified = luamin.minify(lua);

		res.writeHead(200, { 'Content-Type': 'text/plain' });
		res.write(minified);
		res.end();
	});

}).listen(process.env.PORT);
def breadth_first_search(adj_matrix, source_index, end_index)
  node_queue = [source_index]

  loop do
    curr_node = node_queue.pop

    return false if curr_node == nil
    return true if curr_node == end_index

    children = (0..adj_matrix.length-1).to_a.select do |i| 
      adj_matrix[curr_node][i] == 1
    end

    node_queue = children + node_queue
  end
end

def get_nex_dot current_dot, white_dotes
	x1 = current_dot[0].to_f
	y1 = current_dot[1].to_f
	min_dest = nil
	coords = nil
	(white_dotes - @dots_in_line).each do |dot|
		x2 = dot[0].to_f
		y2 = dot[1].to_f
		d = Math.sqrt((x2-x1)**2 + (y2-y1)**2)
		if !min_dest || d < min_dest
			min_dest = d
			coords = dot
		end
	end

	if @dots_in_line.length + 1 == white_dotes.length
		x_first = @dots_in_line.first[0]
		y_first = @dots_in_line.first[1]
		x_free = (white_dotes - @dots_in_line).first[0]
		y_free = (white_dotes - @dots_in_line).first[1]

		d_current_last = Math.sqrt((x_free-x_first)**2 + (y_free-y_first)**2)
		if d_current_last < min_dest
			@dots_in_line.unshift([x_free, y_free])
		end
	end

	coords
end

def draw_a_star_path map_with_a_b_points, start_point_coords, end_point_coords
	map_with_a_b_points[start_point_coords[1]][start_point_coords[0]] = 'A'
	map_with_a_b_points[end_point_coords[1]][end_point_coords[0]] = 'B'

	# Loading external file with the dungeon
	dungeon = map_with_a_b_points.reverse.map do |row_y|
		row_y = row_y.map{|point| r = " "; r = "#" if point == "excluded"; r = point if point == "A" || point == "B";	r}
		row_y.join()
	end
	dungeon = dungeon.join("\n")

	# Creating the graph
	graph = Graph.new(dungeon)
	# Creating the astar object
	astar = ASTAR.new(graph.start, graph.stop)
	# Performing the search
	path  = astar.search

	graph.to_s(path)
end

def add_path_to_map map_with_current_path, map_with_all_paths
	normalized_map_with_current_path = map_with_current_path.reverse.map{|y_row| y_row.split('')}
	lines_y = []
	(0..(map_with_all_paths.length-1)).to_a.each do |y|
		line_x = []
		(0..(map_with_all_paths[y].length-1)).to_a.each do |x|
			point = map_with_all_paths[y][x]
			if normalized_map_with_current_path[y][x].match(/[AB\*]/)
				point = '*'
			end
			if map_with_all_paths[y][x] == '*'
				point = '*'
			end
			line_x.push(point)
		end
		lines_y.push(line_x)
	end
	lines_y
end

iSqs = "(73,61),(49,125),(73,110),(98,49),(126,89),(68,102),(51,132),(101,123),(22,132),(71,120),(97,129),(118,76),(85,100),(88,22),(84,144),(93,110),(96,93),(113,138),(91,52),(27,128),(84,140),(93,143),(83,17),(123,85),(50,74),(139,97),(101,110),(77,56),(86,23),(117,59),(133,126),(83,135),(76,90),(70,12),(12,141),(116,87),(102,76),(19,138),(86,129),(86,128),(83,60),(100,98),(60,105),(61,103),(94,99),(130,124),(141,132),(68,84),(86,143),(72,119)"

oSqs = "(145,82),(20,65),(138,99),(38,137),(85,8),(125,104),(117,48),(57,48),(64,119),(3,25),(40,22),(82,54),(121,119),(1,34),(43,98),(97,120),(10,90),(15,32),(41,13),(86,40),(3,83),(2,127),(4,40),(139,18),(96,49),(53,22),(5,103),(112,33),(38,47),(16,121),(133,99),(113,45),(50,5),(94,144),(16,0),(93,113),(18,141),(36,25),(56,120),(3,126),(143,144),(99,62),(144,117),(48,97),(69,9),(0,9),(141,16),(55,68),(81,3),(47,53)"


white_dotes = iSqs.split(/\(([0-9]+,[0-9]+)\)/).reject{|i| i == ','}.reject{|i| i.empty?}.map{|i| i.split(',').map{|e| e.to_i}}
black_dotes = oSqs.split(/\(([0-9]+,[0-9]+)\)/).reject{|i| i == ','}.reject{|i| i.empty?}.map{|i| i.split(',').map{|e| e.to_i}}

max_y = 0
max_x = 0
white_dotes.each{|i| max_y = i[1] if max_y < i[1]}
black_dotes.each{|i| max_y = i[1] if max_y < i[1]}
white_dotes.each{|i| max_x = i[0] if max_x < i[0]}
black_dotes.each{|i| max_x = i[0] if max_x < i[1]}
invert_white_dotes = white_dotes.map{|i| [i[0],max_y-i[1]]}
invert_black_dotes = black_dotes.map{|i| [i[0],max_y-i[1]]}

# (0..max_y).to_a.each do |y|
# 	str = ""
# 	(0..max_x).to_a.each do |x|
# 		if invert_white_dotes.include? [x,y]
# 			str += "*"
# 		elsif black_dotes.include? [x,y]
# 			str += "-"
# 		else
# 			str += " "
# 		end
# 	end
# 	puts str
# end
@dots_in_line = []

start_point = white_dotes.first
@dots_in_line.push(start_point)

while @dots_in_line.length < white_dotes.length
	dot = get_nex_dot(start_point, white_dotes)
	if dot
		@dots_in_line.push(dot)
	end
end

line_y = []
(0..max_y).to_a.each do |y|
	line_x = []
	(0..max_x).to_a.each do |x|
		if !white_dotes.include?([x,y]) and !black_dotes.include?([x,y])
			line_x.push('unset')
		elsif white_dotes.include?([x,y])
			line_x.push('included')
		elsif black_dotes.include?([x,y])
			line_x.push('excluded')
		end
	end
	line_y.push(line_x)
end

result =  {
	:line => @dots_in_line,
	:coords => line_y
}

#Берем первые 2 точки линии и строим между ними маршрут. Пока только это успел. Итоговая карта помещена в файл map.txt. Маршрут отмечен звездочками.

# Loading all other scripts
%w|./node.rb ./graph.rb ./astar.rb|.each do |l|
  raise LoadError,
    "cannot load required file #{l}" unless load l
end

line_dots = result[:line].clone
end_point_coords = line_dots.shift
map_with_all_paths = result[:coords].clone

while !line_dots.empty?
	start_point_coords = end_point_coords
	end_point_coords = line_dots.shift

	
	map_with_current_path = draw_a_star_path map_with_all_paths, start_point_coords, end_point_coords
	map_with_all_paths = add_path_to_map map_with_current_path, map_with_all_paths
end

#Преобразуем массив для записи в файл
printed_to_file_map = map_with_all_paths.reverse.map do |row_y|
	row_y = row_y.map{|point| r = " "; r = "#" if point == "excluded"; r = "*" if point == "*";	r}
	row_y.join()
end
printed_to_file_map = printed_to_file_map.join("\n")

f = File.new("map.txt", "w")
f.write(printed_to_file_map)
f.close

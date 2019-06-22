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

white_dotes.each do |wd1|
	x1 = wd1[0].to_f
	y1 = wd1[1].to_f
	white_dotes.each do |wd2|
		x2 = wd2[0].to_f
		y2 = wd2[1].to_f
		d = Math.sqrt((x2-x1)**2 + (y2-y1)**2)
	end
end